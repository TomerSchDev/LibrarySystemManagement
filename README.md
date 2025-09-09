# Library Management System

A comprehensive WPF desktop application for managing library operations, built with C# and .NET. This system provides role-based access control, book inventory management, member tracking, and borrowing/returning functionality.

## Features

Core Functionality:
- Book Management: Add, edit, delete, and search books with tracking of available copies
- Member Management: Register and manage library members with contact information
- Borrow/Return System: Issue books to members and process returns with due date tracking
- Search Functionality: Global search across books and members
- Reporting System: Track all system activities with severity levels and user actions
- Data Export: Export data to multiple formats (CSV, PDF, Email)

Security & Access Control:
- Role-Based Authentication: Three user roles with different permission levels:
    - Admin: Full system access including user management and deletion operations
    - Librarian: Can manage books, members, and handle borrowing/returning
    - Member: Limited access to viewing and basic operations
- Activity Logging: All critical actions are logged with severity levels (INFO, LOW, MEDIUM, HIGH, CRITICAL)
- Session Management: Secure session handling with permission verification

## Technical Stack

- Framework: .NET with WPF (Windows Presentation Foundation)
- Database: SQLite with Entity Framework
- Language: C#
- Architecture: MVVM (Model-View-ViewModel) pattern
- Export Libraries:
    - PdfSharpCore for PDF generation
    - MailKit for email functionality
    - Built-in CSV export

## Project Structure
```
Library_System_Management/
├── Models/                             # Data models and entities
│   ├── Book.cs
│   ├── BorrowedBook.cs
│   ├── IExportable.cs
│   ├── Member.cs
│   ├── Report.cs
│   ├── User.cs
│   └── ViewModels/                     # View-specific models
│       └── BorrowedBookView.cs
├── Services/                           # Business logic layer
│   └── ExportServices/                 # Data export functionality
│       ├── CsvExportService.cs
│       ├── IDataExportService.cs
│       ├── MailExportService.cs
│       └── PdfExportService.cs
├── Views/                              # WPF windows and UI
│   ├── LoginWindow.xaml
│   ├── DashboardWindow.xaml
│   ├── BookWindow.xaml
│   ├── MembersWindow.xaml
│   ├── IssueReturnWindow.xaml
│   └── PopUpDialogs/                   # Dialog windows
│       ├── AddBorrowWindow.xaml
│       ├── AddEditBookWindow.xaml
│       ├── AddEditMemberWindow.xaml
│       ├── AddIssueBook.xaml
│       ├── addNewUser.xaml
│       └── EmailPromptWindow.xaml
├── Database/                           # Database context and migrations
│   └── DatabaseManager.cs
├── Helpers/                            # Utility classes
│   ├── ConfigHelper.cs
│   ├── FileRetriever.cs
│   └── RelayCommand.cs
└── Resources/                          # Contains SQLite database file (LibraryDB.sqlite)
```
## Installation & Setup

Prerequisites:
- .NET 6.0 or higher
- Visual Studio 2022 or compatible IDE
- Windows OS (for WPF support)

Configuration:
1. Clone the repository
   2. Create an appsettings.local.json file in the project root with SMTP configuration for email exports:
```
    {
      "Smtp": 
      {
          "Username": "your-email@gmail.com",
          "Password": "your-app-password",
          "Host": "smtp.gmail.com",
          "Port": 587
      }
    }
```
3. Build the solution to restore NuGet packages
4. Run the application

## Usage

Login:
- Start the application and log in with your credentials. The system supports multiple user roles with different access levels.

Main Dashboard:
After login, you'll see the main dashboard with options based on your role:
- Search Menu: Global search for books and members
- Manage Books: Add, edit, view book inventory
- Manage Members: Register and manage library members
- Issue/Return Books: Handle book borrowing and returns
- Reports: View system activity logs (permission-based)
- Users: Manage system users (Admin only)

Key Operations:

Issuing a Book:
1. Navigate to "Issue/Return Books"
2. Click "Issue new Borrow"
3. Select member and book
4. Set return date
5. Confirm the transaction

Returning a Book:
1. Go to "Issue/Return Books"
2. Find the borrowed book in the current borrows list
3. Click "Return" or double-click to open details
4. Confirm the return

Exporting Data:
- Most data grids support export functionality:
    - Click the "Export" button on any data view
    - Choose export format (CSV, PDF, or Email)
    - Specify filename and confirm

## Security Features

- Permission Checking: Every sensitive operation validates user permissions
- Activity Logging: All actions are logged with timestamps and user information
- Session Management: Secure session handling with role verification
- Data Validation: Input validation and error handling throughout

## Database Schema

The system uses SQLite with the following main tables:
- Users: System users with roles and credentials
- Books: Book inventory with quantities
- Members: Library member information
- BorrowedBooks: Transaction records for borrowing
- Reports: Activity logs and system events

## Contributing

This is an educational/demonstration project. Feel free to fork and modify for your needs.

## License

![MIT License](https://img.shields.io/badge/license-MIT-green)

## Support

For issues or questions, please create an issue in the repository.
