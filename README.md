# E-Commerce Web Application

**Live URL:** http://40.66.42.32

---

## How to Use the Application
**New users can also self-register** by clicking "Create one" on the login page.
Registered users get a regular user account automatically.

### Login Credentials

| Role         | Username | Password      |
|--------------|----------|---------------|
| Admin        | admin    | SaifAdmin2024 |
| Regular User | Rehan    | Saif@123      |
| Regular User | David    | Saif@123      |
| Regular User | Göksu    | Saif@123      |

### As a Regular User
1. Go to http://40.66.42.32
2. Click **Sign In** and login with Rehan / Saif@123
3. Browse items on the home page - use category filters to narrow results
4. Click any item to view its details
5. Submit a rating (1-5 stars) and write a review on the item page
6. Click **My Profile** in the navbar to see your reviews and average rating

### As Admin
1. Login with admin / SaifAdmin2024
2. You are redirected to the Admin Panel automatically
3. Use **+ Add Item** to create new items with category-specific fields
4. Use **+ Add User** to create new user accounts
5. Use **Delete** buttons to remove items or users, all related ratings and reviews are automatically cleaned up

---

## Design Decisions

### Programming Language and Framework
ASP.NET Core 8 MVC with C# was chosen for three reasons:
- It is the stack I am most proficient in, allowing me to focus on architecture rather than syntax
- Strong typing and dependency injection make the codebase maintainable and testable
- My existing experience with Azure deployment (from prior projects) meant I could move faster on infrastructure

### General Architecture

**Two MongoDB collections only — Items and Users.**

This directly satisfies the assignment requirement to use the fewest collections possible by taking advantage of MongoDB's flexible schema:

- `Items` collection: Each document contains embedded `Ratings` and `Reviews` arrays directly inside the item. No separate ratings or reviews collection needed.
- `Users` collection: Each user document contains an embedded `Reviews` array (a copy of their reviews for the profile page) and their average rating.

This denormalized design means reads are fast — fetching an item gives you all its ratings and reviews in a single database call.

**Category-specific fields** (BatteryLife, Age, Size, Material) are stored as nullable fields on every item document. MongoDB's dynamic schema means GPS watches simply have a BatteryLife value while other categories leave it null — no separate tables or joins required.

**Authentication** uses ASP.NET Core Cookie Authentication with BCrypt password hashing. Passwords are never stored in plain text — only the BCrypt hash is persisted in MongoDB.

**Cascade operations** are handled in the service layer:
- Deleting an item removes that item's ratings and reviews from all affected user documents
- Deleting a user removes that user's ratings and reviews from all affected item documents, and recalculates average ratings

### Deployment Stack
- **Azure VM** (Ubuntu 24.04) — application server
- **Nginx** — reverse proxy forwarding port 80 to the .NET app on port 5000
- **systemd** — service management, auto-restarts the app on crash or reboot
- **GitHub Actions** — CI/CD pipeline that publishes and deploys on every push to main
- **MongoDB Atlas** — cloud-hosted NoSQL database (M0 free tier)

---

## Database Population

The application is seeded with:
## Seeded Data
- 8 items across all 5 categories (Vinyls, Antique Furniture, GPS Sport Watches,
  Running Shoes, Camping Tents)
- 3 regular users and 1 admin pre-seeded in the database
- New users can register directly through the app
- Every item has been rated and reviewed at least once

---

## Technology Stack Summary

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core 8 MVC (C#) |
| Database | MongoDB Atlas (NoSQL, DBaaS) |
| Authentication | Cookie Authentication + BCrypt |
| Frontend | Razor Views + Custom CSS |
| Server | Azure VM (Ubuntu 24.04) |
| Reverse Proxy | Nginx |
| Service Manager | systemd |
| CI/CD | GitHub Actions |
