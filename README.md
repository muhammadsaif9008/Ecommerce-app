# ECommerce Web Application

## Live URL
http://40.66.42.32

## Login Credentials
- Admin: username `admin`, password `SaifAdmin2024`
- User: username `alice`, password `alice123`

## Tech Stack
- **Framework:** ASP.NET Core 8 MVC (C#)
- **Database:** MongoDB Atlas (NoSQL)
- **Deployment:** Azure VM (Ubuntu 22.04)
- **Web Server:** Nginx reverse proxy
- **CI/CD:** GitHub Actions

## Architecture
The application follows MVC pattern with two MongoDB collections — Items and Users. Ratings and reviews are embedded directly inside Item documents taking advantage of MongoDB's flexible schema.

## Design Decisions
- ASP.NET Core 8 chosen for strong typing and industry demand
- MongoDB chosen for flexible schema — category-specific fields like BatteryLife, Age, Size are nullable
- Azure VM chosen over App Service for full infrastructure control
- Cookie authentication for session management
- BCrypt for password hashing

## Features
- Browse items with category filtering
- Admin panel — add/remove items and users
- Regular users — rate and review items
- User profile page showing reviews and average rating
