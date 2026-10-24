# Inventory Management System

A desktop application for managing products, inventory, categories, and user access, built with C# Windows Forms and SQL Server.

## Features

- **Product Management:** Add, edit, delete, search, and filter products.
- **Category Management:** Manage product categories.
- **User Authentication:** Login and initial administrator setup.
- **Role-Based Access Control:** Separate permissions for Admin and Viewer roles.
- **Secure Password Storage:** Hash passwords using PBKDF2.
- **Audit Logging:** Track product and category operations.
- **Database Integration:** Access SQL Server using parameterized ADO.NET queries.

## Tech Stack

- C#
- Windows Forms
- .NET Framework 4.8
- SQL Server
- ADO.NET
- Layered Architecture

## Project Structure

| Project                       | Responsibility                           |
| ----------------------------- | ---------------------------------------- |
| `InventoryManagement.Core`    | Entities, interfaces, and core models    |
| `InventoryManagement.Data`    | SQL Server data access and repositories  |
| `InventoryManagement.Service` | Business logic, validation, and security |
| `InventoryManagement.UI`      | Windows Forms user interface             |

## Getting Started

### Prerequisites

- Visual Studio with .NET Framework 4.8 development support
- SQL Server

### Setup

1. Clone the repository:

   ```bash
   git clone https://github.com/Aya-Selim/InventoryManagementSystem.git
   ```
