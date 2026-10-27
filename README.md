
# 🎟️ QueueWise
### Smart Queue Management System

> Smarter Queues. Better Service. Less Waiting.

QueueWise is a web-based queue management system built using **C# and ASP.NET Core**.

The application helps organizations manage customer queues, issue digital tickets, and track service progress through a responsive dashboard.

Designed as an educational prototype for universities, offices, and service centers.

---

## 🚀 Features

- 🎫 Automatic queue ticket generation
- 🏢 Multiple service departments
- 👥 Customer queue management
- 🔄 Waiting, Serving, and Completed statuses
- ⚡ Live updates using Server-Sent Events
- ⏱️ Estimated waiting-time calculation
- 📊 Responsive staff dashboard
- 💾 Local JSON data persistence
- 🌐 RESTful API endpoints

## 🛠️ Technology Stack

| Technology | Purpose |
|---|---|
| C# | Core programming language |
| ASP.NET Core 8 | Backend web framework |
| Minimal APIs | HTTP API endpoints |
| Server-Sent Events | Live browser updates |
| JSON | Data persistence |
| HTML5 | Frontend structure |
| CSS3 | Styling |
| JavaScript | UI interactions |
| GitHub Actions | Automated build workflow |

## 📁 Project Structure

```text
queuewise-csharp/
├── src/
│   └── QueueWise/
│       ├── Program.cs
│       ├── QueueWise.csproj
│       └── wwwroot/
│           └── index.html
├── scripts/
│   └── smoke-test.sh
├── .github/
│   └── workflows/
│       └── ci.yml
├── .gitignore
├── LICENSE
└── README.md
```

## ⚙️ Installation

### Requirements

- .NET 8 SDK
- Git
- Modern web browser

### Clone Repository

```bash
git clone https://github.com/YOUR_USERNAME/queuewise-csharp.git
cd queuewise-csharp
```

### Run Application

```bash
dotnet run --project src/QueueWise/QueueWise.csproj
```

Open:

```text
http://127.0.0.1:5090
```

## 🧭 System Workflow

```text
Customer
   |
   v
Select Service
   |
   v
Generate Ticket
   |
   v
Waiting Queue
   |
   v
Staff Calls Next
   |
   v
Service In Progress
   |
   v
Service Completed
```

## 🏢 Available Services

- Registration
- IT Support
- Student Affairs

## 🔌 API Endpoints

| Method | Endpoint | Function |
|---|---|---|
| GET | /api/services | Get services |
| GET | /api/tickets | Get tickets |
| POST | /api/tickets | Generate ticket |
| POST | /api/services/{service}/call-next | Call next customer |
| POST | /api/tickets/{id}/complete | Complete service |
| GET | /api/events | Live update stream |

## 🧪 Build

```bash
dotnet build src/QueueWise/QueueWise.csproj
```

## 🧠 Waiting Time Estimation

QueueWise calculates an estimated waiting time using completed service durations.

The current implementation uses a simple average-based estimation method rather than artificial intelligence.

## 🔮 Future Enhancements

- [ ] Admin and staff authentication
- [ ] QR code ticket verification
- [ ] Online appointment booking
- [ ] SQL Server database integration
- [ ] Email and SMS notifications
- [ ] Queue analytics dashboard
- [ ] Advanced waiting-time prediction

## ⚠️ Project Status

Educational MVP.

The current version uses local JSON storage and does not include authentication. It should not be used for real customer data without additional security and privacy improvements.

## 🤝 Contributions

Feedback, issues, and pull requests are welcome.

## 📜 License

Licensed under the MIT License.

---

### ⭐ Support

Give this repository a star if you like the project!

**Built with C# | ASP.NET Core | Web Development**
