Inventory Management System
A desktop application for managing products, inventory, categories, and user access using C# Windows Forms and SQL Server.
Features
- Product Management: Add, edit, delete, search, and filter products.
- Category Management: Manage product categories.
- User Authentication: Login and initial administrator setup.
- Role-Based Access Control: Admin and Viewer roles with different permissions.
- Secure Password Storage: Password hashing using PBKDF2.
- Audit Logging: Track product and category operations.
- Database Integration: SQL Server access using parameterized ADO.NET queries.
Tech Stack
- C#
- Windows Forms
- .NET Framework 4.8
- SQL Server
- ADO.NET
- Layered Architecture
Project Structure
Project	Responsibility
InventoryManagement.Core	Entities, interfaces, and core models
InventoryManagement.Data	SQL Server access and repositories
InventoryManagement.Service	Business logic, validation, and security
InventoryManagement.UI	Windows Forms user interface

Getting Started
1. Clone the repository.
2. Open InventoryManagementSolution.sln in Visual Studio.
3. Create the InventoryDb database and configure its tables.
4. Update the InventoryDb connection string in InventoryManagement.UI/App.config to match your SQL Server instance.
5. Build and run the application.
Requirements: Visual Studio with .NET Framework 4.8 development support and SQL Server.
Notes
The application uses Windows Authentication for SQL Server by default. Make sure the configured Windows account has the required database permissions.
