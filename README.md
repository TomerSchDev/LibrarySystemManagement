# Library Management System

A comprehensive WPF desktop application for managing library operations, built with C# and .NET—**now supporting a remote database via REST API**. The system provides role-based access control, book inventory management, member tracking, borrowing/return functionality, and real-time, multi-client operation using a central server.

---

## Features

**Core Functionality:**
- Book Management: Add, edit, delete, and search books with tracking of available copies (via REST API)
- Member Management: Register and manage library members with contact information
- Borrow/Return System: Issue books to members and process returns with due date tracking
- Search Functionality: Global search across books and members
- Reporting System: Track all system activities with severity levels and user actions
- Data Export: Export data to multiple formats (CSV, PDF, Email)

**Security & Access Control:**
- Role-Based Authentication
    - Admin: Full access, including user management and deletions
    - Librarian: Manage books, members, and borrowing/return
    - Member: Limited to viewing and basic operations
- Activity Logging: All critical actions logged with severity levels (INFO, LOW, MEDIUM, HIGH, CRITICAL)
- Session Management: Secure session handling with permission verification

---

## What’s New: Remote DB & REST API Support

> **The system now operates with a remote database, accessed only via RESTful API (ASP.NET Core Web API).**  
> All data operations (books, members, users, borrows, etc.) are performed by sending secure HTTP requests to the API. This enables multi-user, distributed access and future scalability.

**Key Benefits:**
- No local database needed on client machines
- Central source of truth for all library operations and synchronization
- Server-side business logic, validation, and security
- Easier cloud-based or networked deployment

---

## Technical Stack

- **Client:** .NET with WPF (Windows Presentation Foundation)
- **Server:** ASP.NET Core Web API (RESTful, stateless)
- **Database:**
    - Remote SQL database (SQL Server, PostgreSQL, etc.) managed by the API server
    - *Legacy mode:* Local SQLite (now deprecated)
- **Language:** C#
- **Architecture:**
    - Client: MVVM (Model-View-ViewModel)
    - Server: RESTful MVC
- **Export Libraries:** PdfSharpCore (PDF), MailKit (email), built-in CSV export

---

## Project Structure

```
Library_System_Management/
├── LibrarySystemModels/ # Shared models, logic, helpers
│ ├── Database/Helpers/Models/Services/
│ └── LibrarySystemModels.csproj
├── LibraryRestApi/ # REST API back-end
│ ├── Controllers/ # BooksController, MembersController, etc.
│ ├── appsettings.json
│ └── LibraryRestApi.csproj
├── MainProject/ # WPF Client
│ ├── Views/ # Windows, dialogs (XAML)
│ ├── ExportServices/
│ ├── App.xaml
│ └── Library_System_Management.csproj
├── TestsLibrary/ # Unit, integration tests
│ ├── RESTAPI_Tests/
│ └── ServicesTests/
├── Resources/ # (Legacy) SQLite DB, assets, etc.
├── Dockerfile / compose.yaml # For API deployment (optional)
├── README.md
└── LICENSE
```

---

## Installation & Setup

**Prerequisites:**
- .NET 6.0 or higher
- Visual Studio 2022 or compatible IDE
- Windows OS (for WPF app)
- Docker (optional, for API deployment)

### 1. Clone the Repository

### 2. REST API Server Setup

- Go to `LibraryRestApi` directory and build the project
- Set DB connection string and settings in `appsettings.json` or `appsettings.Development.json`
- Start the API server
    - Via Visual Studio, `dotnet run`, or Docker
    - Example:  
      `dotnet run --project LibraryRestApi/LibraryRestApi.csproj`
- *(Optional)* Use `Dockerfile` and `compose.yaml` for containerized deployment

### 3. Client (WPF) Setup

- In `MainProject`, create a file named `appsettings.local.json` with contents:
    ```
    {
      "Smtp": {
        "Username": "your-email@gmail.com",
        "Password": "your-app-password",
        "Host": "smtp.gmail.com",
        "Port": 587
      }
    }
    ```
- Build the entire solution to restore NuGet packages
- Ensure the API server is running and reachable before launching the WPF app

**Note:**
- The client app will not function unless the API server is running and the URL configuration is correct.
- Local SQLite support is deprecated in favor of the REST API backend.

---

## Usage

- **Login:** Start the WPF app and log in (credentials validated via API)
- **Main Dashboard:** View changes according to your user role
- **Manage Books/Members, Issue/Return Books, Export Data:**  
  All operations are performed as HTTP requests through the central server

---

## Security Features

- All access controls enforced by the server
- Activity logging on the API for all key actions
- Secure session/token management
- Data validation both client- and server-side

---

## Database Schema

Managed by the API server; main tables include:
- Users: Credentials and roles
- Books: Library inventory
- Members: Member details
- BorrowedBooks: Book loans/returns
- Reports: Activity/event logs

> **All modifications happen through the REST API—not directly from the client.**

---

## Contributing

This is an educational/demonstration project. You are welcome to fork, modify, and extend!

---

## License

![MIT License](https://img.shields.io/badge/license-MIT-green)

---

## Support

Please open an issue in the repository for any help or questions.

