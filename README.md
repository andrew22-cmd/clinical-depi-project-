# 🏥 Clinic Management System

A clinic management web application built with **ASP.NET Core MVC** and **SQL Server**, providing dedicated workflows for managers, doctors, secretaries, and patients.

The application includes an **Arabic, right-to-left interface** for managing appointments, doctor schedules, and patient records.

---

## ✨ Features

### 👨‍💼 Manager
- Add, update, and delete staff members.
- View and manage users.
- Manage doctor schedules and appointment slots.
- View reservations by date.
- Access dashboard statistics.

### 👨‍⚕️ Doctor
- View weekly schedules.
- View today's patients.
- Record diagnoses and patient visit notes.

### 👩‍💻 Secretary
- Create and cancel reservations.
- Browse doctors by specialty.
- Check available appointment slots.
- View doctor schedules and reservation statistics.

### 👤 Patient
- Register an account and confirm their email.
- View and update profile information.
- Browse specialties and doctors.
- Book and cancel appointments.
- View reservation history and checkup records.

### 🔐 Account Management
- Session-based login and logout.
- Email confirmation.
- Resend email confirmation.
- Password reset through email.

---

## 🛠️ Technology Stack

| Category | Technologies |
|----------|--------------|
| Language | C# |
| Framework | ASP.NET Core MVC — .NET 8 |
| Database | Microsoft SQL Server |
| ORM | Entity Framework Core 8 |
| Frontend | Razor Views, HTML, CSS, JavaScript |
| UI Libraries | Bootstrap, jQuery, Toastr |
| Email | SMTP |
| Architecture | MVC, Service Layer, Repository Pattern |
| Dependency Management | Built-in Dependency Injection |

---

## 🏗️ Application Architecture

The application separates presentation, business logic, and data access:

- **Controllers** handle requests and return views or JSON responses.
- **Services** implement application workflows and business logic.
- **Repositories** handle database operations.
- **Entity Framework Core** connects the application to SQL Server.
- **Razor Views** render the user interface.

---

## 📂 Project Structure

| Path | Description |
|------|-------------|
| `clinicsystem.sln` | Visual Studio solution |
| `clinicsystem/Controllers/` | Account and role-specific controllers |
| `clinicsystem/Models/` | Database entities |
| `clinicsystem/Data/` | Database context and startup seeder |
| `clinicsystem/Repositories/` | Repository interfaces and implementations |
| `clinicsystem/Services/` | Business logic and email services |
| `clinicsystem/ViewModels/` | View models |
| `clinicsystem/ViewsModels/` | Additional view models |
| `clinicsystem/Views/` | Razor views and shared layouts |
| `clinicsystem/Migrations/` | Entity Framework Core migrations |
| `clinicsystem/wwwroot/` | Static frontend assets |
| `SeedData.sql` | Alternative sample dataset |
| `FullResetAndSeed.sql` | Database data reset and sample-data script |

---

## 🗃️ Main Entities

- Users
- Doctors
- Patients
- Specialities
- Doctor Schedules
- Doctor Schedule Slots
- Reservations
- Patient Notes
- Email Notifications

---

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK.
- Microsoft SQL Server or SQL Server Express.
- Git.
- An SMTP account for email functionality.
- Optional: Visual Studio 2022 with the ASP.NET and web development workload.

### 1. Clone the Repository

```bash
git clone https://github.com/andrew22-cmd/clinical-depi-project-.git
cd clinical-depi-project-
```

### 2. Restore Dependencies

```bash
dotnet restore clinicsystem.sln
```

### 3. Configure Local Settings

Initialize development secrets:

```bash
dotnet user-secrets init --project clinicsystem/clinicsystem.csproj
```

Set your SQL Server connection string:

```bash
dotnet user-secrets set "ConnectionStrings:constring" "Server=localhost\SQLEXPRESS;Database=clinicsystem;Integrated Security=True;TrustServerCertificate=True;" --project clinicsystem/clinicsystem.csproj
```

Replace the server and authentication settings to match your local SQL Server installation.

Configure your SMTP settings:

```bash
dotnet user-secrets set "EmailSettings:SmtpServer" "smtp.example.com" --project clinicsystem/clinicsystem.csproj
dotnet user-secrets set "EmailSettings:SmtpPort" "587" --project clinicsystem/clinicsystem.csproj
dotnet user-secrets set "EmailSettings:SenderName" "ClinicSystem" --project clinicsystem/clinicsystem.csproj
dotnet user-secrets set "EmailSettings:SenderEmail" "your-email@example.com" --project clinicsystem/clinicsystem.csproj
dotnet user-secrets set "EmailSettings:Username" "your-email@example.com" --project clinicsystem/clinicsystem.csproj
dotnet user-secrets set "EmailSettings:Password" "YOUR_SMTP_PASSWORD" --project clinicsystem/clinicsystem.csproj
dotnet user-secrets set "EmailSettings:EnableSsl" "true" --project clinicsystem/clinicsystem.csproj
```

Use your provider's SMTP server, port, and TLS settings. Do not commit real credentials.

### 4. Prepare the Database

> **Current setup requirement:** The committed migrations do not cover all current model properties, including email-confirmation fields and consultation fees. A fresh development database needs an additional migration.

Install the Entity Framework Core CLI if it is not already installed:

```bash
dotnet tool install --global dotnet-ef --version 8.0.26
```

Generate a migration to synchronize the current models:

```bash
dotnet ef migrations add SyncCurrentModels --project clinicsystem/clinicsystem.csproj
```

Review the generated migration before applying it.

In **PowerShell**, select the Development environment so the connection string can be read from user secrets, then update the database:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet ef database update --project clinicsystem/clinicsystem.csproj
```

In **Bash**:

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet ef database update --project clinicsystem/clinicsystem.csproj
```

### 5. Run the Application

```bash
dotnet run --project clinicsystem/clinicsystem.csproj --launch-profile http
```

Open:

**http://localhost:5100**

Login page:

**http://localhost:5100/Account/Login**

---

## 🧪 Demo Accounts

On startup, the application applies migrations and seeds sample data if the `Users` table is empty.

| Role | Email | Password |
|------|-------|----------|
| Manager | `manager@gmail.com` | `123456` |
| Secretary | `secretary@gmail.com` | `123456` |
| Doctor | `doctor1@gmail.com` | `123456` |
| Patient | `patient1@gmail.com` | `123456` |

> The startup seeder does not mark these accounts as email-confirmed, but login requires email confirmation.

For a **local demo database only**, run this SQL after startup seeding to enable the four accounts above:

```sql
USE [clinicsystem];

UPDATE [Users]
SET [EmailConfirmed] = 1
WHERE [Email] IN (
    'manager@gmail.com',
    'secretary@gmail.com',
    'doctor1@gmail.com',
    'patient1@gmail.com'
);
```

To test actual email confirmation, register using an email address you control.

---

## 🌱 Sample Data

The repository includes two optional SQL scripts:

| Script | Purpose |
|--------|---------|
| `SeedData.sql` | Inserts an alternative sample dataset using fixed IDs |
| `FullResetAndSeed.sql` | Deletes existing application data, resets identity counters, and inserts sample data |

These scripts use different accounts from the startup seeder.

> **Warning:** `FullResetAndSeed.sql` deletes existing application data. Use it only with a disposable development database. Review fixed-ID conflicts before combining either dataset with existing records.

---

## 📝 Development Notes

- Some current model fields require an additional database migration.
- Passwords are currently compared directly; password hashing is needed before production use.
- Session-based role routing needs consistent authorization checks across protected actions.
- Any real SMTP credentials committed to the repository should be revoked and replaced.
- Some manager dashboard metrics are currently hard-coded.
- The manager `Reports` action references a view that is not included.
- The shared layout references `site.css` and `site.js`, while the tracked custom assets use different filenames.
- No automated test project is currently included.
- These instructions are based on source review; an end-to-end application run has not been verified.

---

## 📄 License

No license file is currently included. Contact the repository owner for reuse or redistribution terms.
