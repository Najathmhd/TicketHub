
# EventsHub – Event Management System

EventsHub is a web-based Event Management System developed for the Metropolitan Cultural Council. The system provides a centralized platform to manage events, ticket bookings, user reviews, and inquiries in a secure and structured environment.

This project demonstrates full-stack development using ASP.NET Core MVC, Entity Framework Core, and SQL Server.

---

## Project Overview

EventsHub allows:

* Guests to browse events and submit inquiries
* Members to book tickets, manage profiles, and submit reviews
* Admins to manage events, venues, categories, tickets, and users

The system follows the MVC (Model-View-Controller) architecture for clean structure and maintainability.

---

## Technologies Used

Backend:

* .NET 8
* ASP.NET Core MVC
* Entity Framework Core
* SQL Server

Frontend:

* Razor Views (.cshtml)
* Bootstrap 5
* HTML5 / CSS3

Security:

* ASP.NET Identity
* Role-Based Access Control (RBAC)
* Password hashing
* Anti-forgery protection

---

## Key Features

* Dynamic event listing and search
* Ticket booking with availability validation
* Verified review system (only booked members can review)
* Inquiry management system
* Admin dashboard for managing system data
* Secure authentication and authorization

---

## Database Structure

Main Tables:

* EventCategory
* Venue
* Member
* Event
* TicketType
* Booking
* BookingDetail
* Payment
* Review
* Inquiry

The database uses proper primary keys, foreign keys, and relationships to ensure data integrity and scalability.

---

## Admin Credentials

Email: [najamhd037@gmail.com](mailto:najamhd037@gmail.com)
Password: Admin@123

Note: Change credentials before deploying to production.

---

## Installation & Setup

### Prerequisites

* .NET 8 SDK (or later)
* Microsoft SQL Server
* Visual Studio 2022 (recommended)

---

### Step 1 – Clone Repository

```
git clone https://github.com/your-username/EventsHub.git
cd EventsHub
```

---

### Step 2 – Configure Database

Open `appsettings.json` and update the connection string if needed.

---

### Step 3 – Apply Database Migrations

```
dotnet ef database update
```

---

### Step 4 – Run Application

```
dotnet run
```

Open browser and navigate to:

```
https://localhost:7154
```

(Use the port shown in your terminal if different.)

---

## Future Improvements

* Integration with Stripe or PayPal payment gateway
* QR code-based ticket validation
* Mobile application development
* Advanced analytics dashboard
* Email and SMS notifications

---

## Purpose of the Project

This system was developed as part of academic coursework and demonstrates practical implementation of MVC architecture, relational database design, secure authentication, and scalable web application development.

