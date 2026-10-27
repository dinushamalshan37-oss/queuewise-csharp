using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls(Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "http://127.0.0.1:5090");
var app = builder.Build();
var store = new QueueStore(Path.Combine(app.Environment.ContentRootPath, "data", "tickets.json"));
var broadcaster = new ChangeBroadcaster();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/services", () => QueueStore.Services);
app.MapGet("/api/tickets", async () => Results.Ok(await store.ListAsync()));
app.MapPost("/api/tickets", async (CreateTicket input) => {
    if (string.IsNullOrWhiteSpace(input.Name) || input.Name.Trim().Length > 80 || !QueueStore.Services.Contains(input.Service))
        return Results.BadRequest(new { error = "Name must be 1–80 characters and service must be valid." });
    var ticket = await store.CreateAsync(input.Name.Trim(), input.Service);
    broadcaster.Publish();
    return Results.Created($"/api/tickets/{ticket.Id}", ticket);
});
app.MapPost("/api/services/{service}/call-next", async (string service) => {
    if (!QueueStore.Services.Contains(service)) return Results.BadRequest(new { error = "Invalid service." });
    var ticket = await store.CallNextAsync(service);
    if (ticket is null) return Results.Conflict(new { error = "No waiting ticket, or a ticket is already being served." });
    broadcaster.Publish();
    return Results.Ok(ticket);
});
app.MapPost("/api/tickets/{id:guid}/complete", async (Guid id) => {
    var ticket = await store.CompleteAsync(id);
    if (ticket is null) return Results.Conflict(new { error = "Ticket not found or not currently being served." });
    broadcaster.Publish();
    return Results.Ok(ticket);
});
// Server-Sent Events: browser receives instant refresh notifications without third-party JS dependencies.
app.MapGet("/api/events", async (HttpContext ctx) => {
    ctx.Response.ContentType = "text/event-stream";
    ctx.Response.Headers["Cache-Control"] = "no-cache";
    var channel = Channel.CreateUnbounded<string>();
    using var subscription = broadcaster.Subscribe(channel.Writer);
    try {
        await ctx.Response.WriteAsync("data: connected\n\n", ctx.RequestAborted);
        await ctx.Response.Body.FlushAsync(ctx.RequestAborted);
        await foreach (var message in channel.Reader.ReadAllAsync(ctx.RequestAborted)) {
            await ctx.Response.WriteAsync($"data: {message}\n\n", ctx.RequestAborted);
            await ctx.Response.Body.FlushAsync(ctx.RequestAborted);
        }
    } catch (OperationCanceledException) { /* Client disconnected. */ }
});
app.Run();

record CreateTicket(string Name, string Service);
record Ticket(Guid Id, string Name, string Service, int Sequence, string Status, DateTime CreatedUtc, DateTime? StartedUtc = null, DateTime? CompletedUtc = null);

sealed class ChangeBroadcaster {
    private readonly ConcurrentDictionary<Guid, ChannelWriter<string>> _connections = new();
    public IDisposable Subscribe(ChannelWriter<string> writer) {
        var key = Guid.NewGuid(); _connections[key] = writer;
        return new Cleanup(() => _connections.TryRemove(key, out _));
    }
    public void Publish() {
        foreach (var writer in _connections.Values) writer.TryWrite("changed");
    }
    private sealed class Cleanup(Action action) : IDisposable { public void Dispose() => action(); }
}

sealed class QueueStore {
    public static readonly string[] Services = ["Registration", "IT Support", "Student Affairs"];
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly string _path;
    private readonly List<Ticket> _tickets;
    public QueueStore(string path) {
        _path = path;
        if (File.Exists(_path)) {
            _tickets = JsonSerializer.Deserialize<List<Ticket>>(File.ReadAllText(_path)) ?? [];
        } else _tickets = [];
    }
    private async Task PersistAsync() {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        var temp = _path + ".tmp";
        await File.WriteAllTextAsync(temp, JsonSerializer.Serialize(_tickets, new JsonSerializerOptions { WriteIndented = true }));
        File.Move(temp, _path, true);
    }
    public async Task<Ticket[]> ListAsync() {
        await _gate.WaitAsync();
        try { return _tickets.OrderByDescending(t => t.CreatedUtc).ToArray(); }
        finally { _gate.Release(); }
    }
    public async Task<Ticket> CreateAsync(string name, string service) {
        await _gate.WaitAsync();
        try {
            int number = _tickets.Where(t => t.Service == service).Select(t => t.Sequence).DefaultIfEmpty(0).Max() + 1;
            var ticket = new Ticket(Guid.NewGuid(), name, service, number, "Waiting", DateTime.UtcNow);
            _tickets.Add(ticket);
            await PersistAsync();
            return ticket;
        } finally { _gate.Release(); }
    }
    public async Task<Ticket?> CallNextAsync(string service) {
        await _gate.WaitAsync();
        try {
            if (_tickets.Any(t => t.Service == service && t.Status == "Serving")) return null;
            int index = _tickets.FindIndex(t => t.Service == service && t.Status == "Waiting");
            if (index < 0) return null;
            var ticket = _tickets[index] with { Status = "Serving", StartedUtc = DateTime.UtcNow };
            _tickets[index] = ticket;
            await PersistAsync();
            return ticket;
        } finally { _gate.Release(); }
    }
    public async Task<Ticket?> CompleteAsync(Guid id) {
        await _gate.WaitAsync();
        try {
            int index = _tickets.FindIndex(t => t.Id == id && t.Status == "Serving");
            if (index < 0) return null;
            var ticket = _tickets[index] with { Status = "Completed", CompletedUtc = DateTime.UtcNow };
            _tickets[index] = ticket;
            await PersistAsync();
            return ticket;
        } finally { _gate.Release(); }
    }
}
