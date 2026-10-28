
# Inventory Management System

A desktop inventory management application built with **C#, Windows Forms, .NET Framework 4.8, and SQL Server**.

[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/Language-C%23-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-CC2927?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server/)

## Features

- **Authentication:** Login, registration, and first-time Admin setup.
- **Role-Based Access Control:** Admin and Viewer roles.
- **Product Management:** Add, edit, search, filter, and soft-delete products.
- **Category Management:** Manage categories with transactional cascading soft deletes.
- **Data Grid:** Telerik RadGridView with filtering and column customization.
- **Reporting:** Low-stock reports using SAP Crystal Reports.
- **Audit Logging:** Track product and category changes.
- **Security:** PBKDF2 password hashing and parameterized SQL queries.

## Tech Stack

- **Language:** C# 7.3+
- **Framework:** .NET Framework 4.8
- **UI:** Windows Forms, Telerik UI for WinForms
- **Database:** Microsoft SQL Server
- **Data Access:** ADO.NET
- **Architecture:** Four-layer architecture
- **Patterns:** Repository and Unit of Work
- **Reporting:** SAP Crystal Reports

## Architecture

The solution separates responsibilities into four projects:

| Project | Responsibility |
|---|---|
| `InventoryManagement.UI` | Windows Forms and presentation |
| `InventoryManagement.Service` | Business logic, validation, and authorization |
| `InventoryManagement.Core` | Entities, enums, and interfaces |
| `InventoryManagement.Data` | ADO.NET repositories and database transactions |

The application uses a manual composition root in `Program.cs` to initialize repositories and services.

## Database

The application uses SQL Server with the following tables:

- `Users` — accounts and roles
- `Categories` — product categories
- `Products` — inventory items
- `AuditLogs` — records of product and category changes

## Getting Started

### Prerequisites

- Visual Studio 2019 or 2022 with the **.NET desktop development** workload.
- .NET Framework 4.8 Developer Pack.
- Microsoft SQL Server.
- Required Telerik UI for WinForms assemblies.
- SAP Crystal Reports runtime compatible with the application architecture.

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/Aya-Selim/InventoryManagementSystem.git
   cd InventoryManagementSystem
   ```

2. Create the `InventoryDb` database and its required tables using your database setup script.

3. Update the connection string in `InventoryManagement.UI/App.config`.

4. Restore NuGet packages and build the solution in Visual Studio.

5. Set `InventoryManagement.UI` as the startup project and run the application.

6. On an empty database, complete the initial Admin setup, then sign in.

## Reporting

The Low Stock Report displays active products with quantities at or below **5 units**. Crystal Reports provides report viewing, printing, and export capabilities.

## Security

- Passwords are stored as PBKDF2-derived hashes with salts.
- SQL queries use parameters to help prevent SQL injection.
- Admin-only operations are protected by authorization checks.
- Soft deletes preserve records for historical tracking.

---

**Note:** Configure your own database connection string and install the required Telerik and Crystal Reports dependencies before running the application.
