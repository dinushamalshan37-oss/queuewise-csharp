🎟️ QueueWise
Better queues. Less waiting. Clearer service workflows.
Smart Queue & Service Token Management | C# | ASP.NET Core 8
A web-based queue management MVP for campus offices and service counters. Generate a ticket, track the queue and update every open dashboard when staff call the next customer.

Status: Educational MVP — local-first  ·  Language: C#  ·  License: MIT
✨ Highlights
Feature	What it does
🎫 Ticket generation	Create sequential ticket numbers per service
🏢 Multiple services	Registration, IT Support and Student Affairs
🔄 Queue workflow	Waiting → Serving → Completed
📣 Call next	Staff can serve tickets in first-in-first-out order
⏱️ Wait estimate	Simple average based on completed service times
⚡ Live updates	Server-Sent Events (SSE) refresh browser views
💾 Persistent storage	Tickets are stored in a local JSON file
📱 Responsive interface	Browser UI that works at different screen sizes


🧰 Tech stack
C# · ASP.NET Core 8 · Minimal APIs · Server-Sent Events · JSON · HTML/CSS/JavaScript · GitHub Actions

No third-party NuGet packages are required.
🚀 Getting started
Prerequisite: Install the .NET 8 SDK.
dotnet --version
git clone https://github.com/YOUR_USERNAME/queuewise-csharp.git
cd queuewise-csharp
dotnet run --project src/QueueWise/QueueWise.csproj
Open http://127.0.0.1:5090 in your browser.
Custom port
On Linux / macOS:
ASPNETCORE_URLS=http://127.0.0.1:7000 dotnet run --project src/QueueWise/QueueWise.csproj
On Windows PowerShell:
$env:ASPNETCORE_URLS="http://127.0.0.1:7000"
dotnet run --project src/QueueWise/QueueWise.csproj
🧭 Typical workflow
Student / Customer        QueueWise API                Staff
       │                       │                       │
       ├── Request ticket ────►│                       │
       │◄── Ticket number ─────┤                       │
       │                       │◄── Call next ─────────┤
       │◄── Live SSE update ───┤                       │
       │                       │◄── Mark completed ────┤
       │◄── Queue updates ─────┤                       │
Services in this MVP: Registration, IT Support and Student Affairs. Each has its own ticket sequence and service queue.
🔌 API reference
Method	Route	Description
GET	/api/services	Available services
GET	/api/tickets	Tickets and statuses
POST	/api/tickets	Issue a ticket
POST	/api/services/{service}/call-next	Call the next waiting ticket
POST	/api/tickets/{id}/complete	Complete a serving ticket
GET	/api/events	Server-Sent Events stream


Example ticket request:
curl -X POST http://127.0.0.1:5090/api/tickets \
  -H 'Content-Type: application/json' \
  -d '{"name":"Nimal","service":"Registration"}'
📁 Project structure
queuewise-csharp/
├── .github/workflows/ci.yml
├── src/QueueWise/
│   ├── Program.cs                 # API, queue logic, persistence
│   ├── QueueWise.csproj
│   └── wwwroot/index.html         # Browser interface
├── scripts/smoke-test.sh
├── .gitignore
└── LICENSE
The app creates src/QueueWise/data/tickets.json locally. Do not commit any real customers' personal information.
✅ Build and verification
dotnet build src/QueueWise/QueueWise.csproj --configuration Release
With the app already running, you can also use the optional API smoke test:
bash scripts/smoke-test.sh
GitHub Actions is configured to run the .NET build. Note: This starter was not compiled in the environment where it was originally generated because the .NET SDK was unavailable. Run the commands above on your machine and resolve any reported issues before showcasing it as fully verified.
🗺️ Roadmap (not implemented yet)
- [ ] Admin/staff authentication and permissions
- [ ] Appointment calendar and QR check-in
- [ ] Database migration to SQLite or SQL Server
- [ ] Audit trails, analytics and reporting
- [ ] Configurable priority and accessibility rules
🔒 Important limitations
This is a trusted local demonstration. Staff actions currently have no authentication, customer names are saved locally in plain JSON, and no real appointment calendar is included. Waiting time is an average-based estimate, not AI prediction. Do not deploy as a real hospital/bank service without security, privacy and concurrency hardening.
📸 Screenshots
Add authentic images of the ticket creation page, live queue display and staff workflow once you have run the app.
🤝 Contributions & license
Suggestions and pull requests are welcome. MIT licensed — see LICENSE.
GitHub description: An ASP.NET Core queue management system with ticket generation, service counters, and real-time updates.
