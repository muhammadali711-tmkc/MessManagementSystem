🍽️ Mess Management System

A comprehensive web-based application built with ASP.NET Core 8 MVC to manage the daily operations, attendance, and billing of a faculty mess. This system automates the calculation of monthly bills based on daily food consumption, fixed water charges, and arrears handling.

 🚀 Key Features

👨‍💼 Admin Portal
User Management: Register new teachers and manage their accounts (Edit/Delete).
Menu Planning: Define weekly menus with specific daily rates (e.g., Biryani = 300 PKR).
Daily Attendance: Mark teachers as "Present" (consumed food) or "Absent".
Automated Billing:
 Calculates food cost based on specific days attended.
    	 Adds fixed monthly water charges.
    	 Automatically carries over unpaid arrears from previous months.
Force Password Change: New users are forced to change their default password upon first login.

 👨‍🏫 Teacher Portal
Dashboard: View current balance, dues, and advance payments.
Attendance History: View a log of all days marked present/absent.
Bill Management: View detailed breakdown of monthly bills.
Online Payment: Integrated **Stripe Payment Gateway** to pay bills securely via Credit/Debit card.

 🛠️ Tech Stack
Framework: ASP.NET Core 8 MVC
Database: SQL Server (Entity Framework Core)
Authentication: JWT (JSON Web Tokens) stored in Secure Cookies + Session Management.
Frontend: Bootstrap 5, Razor Views.
Payment Gateway: Stripe API (Test Mode).

 ⚙️ Setup & Installation

1.  Prerequisites:
     Visual Studio 2022
     SQL Server Management Studio (SSMS)
     .NET 8.0 SDK

2. Database Setup:
     Open `appsettings.json` and update the `ConnectionStrings` with your local SQL Server instance.
     Run the project to let Entity Framework initialize the connection (or run the SQL script provided in `/Database` folder).

3. Stripe Configuration:
     Sign up at [Stripe Dashboard](https://dashboard.stripe.com/).
     Get your **Publishable Key** and **Secret Key**.
     Update `Program.cs` with the Secret Key.
     Update `JazzCashGateway.cshtml` with the Publishable Key.

4. Run the Application:
     Open the solution in Visual Studio.
     Press `F5` or click "Run".
    Admin Credentials: `admin@mess.com` / `admin123` (Example).

🔒 Security Features
Secure Authentication: Uses JWT tokens with `HttpOnly` cookies to prevent XSS attacks.
Role-Based Authorization: Strict middleware checks to prevent Students/Teachers from accessing Admin pages.
Sensitive Data: Passwords should be hashed (recommended for production).

 📸 Screenshots


Login form:





Admin Dashboard:

 
Manage Teachers:




Menu setup:





 Menu listing:




Teacher attendance:






Bill generation :



Bill history:






Teacher’s Dashboard:




Teacher attendance history:






Teacher monthly bills and payment:





Developed by: [Muhammad Ali (2023-CS-711)]
Semester Project: Enterprise Application Development

