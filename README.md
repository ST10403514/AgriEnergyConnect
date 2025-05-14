# AgriEnergyConnect

AgriEnergyConnect is a web application that connects farmers, employees, and green energy experts to promote sustainable agriculture through a marketplace, discussion forum, project collaboration, funding opportunities, and educational resources. Built with ASP.NET Core and SQLite, it supports buying and selling eco-friendly products (e.g., biogas generators for R 3000), proposing sustainable projects, and sharing expertise.

## Table of Contents
- [Features](#features)
- [User Roles](#user-roles)
- [Prerequisites](#prerequisites)
- [Setup Instructions](#setup-instructions)
- [Building and Running the Application](#building-and-running-the-application)
- [Using the Application](#using-the-application)
- [Troubleshooting](#troubleshooting)
- [License](#license)

## Features
- **Marketplace**: Farmers list and sell sustainable products (e.g., solar irrigation systems, biogas generators). All users can browse products, view details, and leave star ratings (1–5, e.g., ★★★★☆). Farmers can place orders.
- **Discussion Forum**: Users discuss organic farming, water conservation, and green energy solutions.
- **Project Collaboration**: Farmers propose sustainability projects (e.g., solar-powered irrigation). Others view and support projects.
- **Funding Opportunities**: Browse grants like the Green Farming Grant to fund sustainable initiatives.
- **Farmer Management**: Employees manage farmer profiles to grow the community.
- **Education Hub**: Access resources on sustainable agriculture and green energy.
- **Authentication**: Role-based access for Farmers, Employees, and Green Energy Experts, with secure login via ASP.NET Core Identity.

## User Roles
AgriEnergyConnect supports three user roles, each with specific functionalities:

1. **Farmer**:
   - **Credentials**: Email: `farmer@example.com`, Password: `Farmer@123`
   - **Functionalities**:
     - List and manage products in the marketplace (e.g., add a biogas generator for R 3000).
     - Leave star ratings (1–5) and comments on products (e.g., “Good system” with ★★★★☆).
     - Place orders for products.
     - Propose sustainability projects (e.g., solar irrigation systems).
     - Participate in the discussion forum.
     - Access educational resources.

2. **Employee**:
   - **Credentials**: Email: `employee@example.com`, Password: `Employee@123`
   - **Functionalities**:
     - Manage farmer profiles (view, edit, delete).
     - Browse the marketplace and view product details, including reviews.
     - Participate in the discussion forum.
     - View projects and funding opportunities.
     - Access educational resources.

3. **Green Energy Expert**:
   - **Credentials**: Email: `expert@example.com`, Password: `Expert@123`
   - **Functionalities**:
     - Share expertise in the discussion forum (e.g., on water conservation).
     - Browse the marketplace and view product reviews.
     - Evaluate funding opportunities (e.g., Green Farming Grant).
     - View projects proposed by farmers.
     - Access educational resources.

## Prerequisites
To set up and run AgriEnergyConnect, ensure you have:
- **Operating System**: Windows, macOS, or Linux
- **.NET SDK**: Version 8.0 or later
- **SQLite**: No separate installation needed (SQLite is embedded)
- **Code Editor**: Visual Studio 2022, Visual Studio Code, or similar
- **Git**: For cloning the repository (optional)
- **Web Browser**: Chrome, Firefox, or Edge for testing

## Setup Instructions
Follow these steps to set up the development environment:

1. **Install .NET SDK**:
   - Download and install the .NET 8.0 SDK from [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download).
   - Verify installation:
     ```
     dotnet --version
     ```
     Expected output: `8.0.x` or later.

2. **Clone the Repository** (if using source control):
   - Run:
     ```
     git clone <https://github.com/ST10403514/AgriEnergyConnect>
     cd AgriEnergyConnect
     ```
   - Alternatively, copy the project files to `C:\Users\<YourUsername>\source\repos\AgriEnergyConnect`.

3. **Install Entity Framework Core CLI** (if not already installed):
   - Run:
     ```
     dotnet tool install --global dotnet-ef
     ```
   - Verify:
     ```
     dotnet ef --version
     ```

4. **Restore Dependencies**:
   - Navigate to the project directory:
     ```
     cd C:\Users\<YourUsername>\source\repos\AgriEnergyConnect
     ```
   - Restore NuGet packages:
     ```
     dotnet restore
     ```

## Building and Running the Application
1. **Apply Database Migrations**:
   - Ensure the SQLite database (`AgriEnergyConnect.db`) is set up:
     ```
     dotnet ef migrations add InitialCreate --context ApplicationDbContext
     dotnet ef database update --context ApplicationDbContext
     ```
   - This creates the database and seeds initial data (10 farmers, 10 products, 10 reviews, etc.).

2. **Build the Application**:
   - Run:
     ```
     dotnet build
     ```
   - Expected output: No errors.

3. **Run the Application**:
   - Start the web server:
     ```
     dotnet run
     ```
   - The app will run at `https://localhost:7220`.

4. **Access the Application**:
   - Open a web browser and navigate to `https://localhost:7220`.
   - The home page displays the AgriEnergyConnect welcome message and features.

## Using the Application
### General Navigation
- **Home Page**: View features like the marketplace, discussion forum, and funding opportunities. Log in or register to access role-specific features.
- **Marketplace**: Browse products (e.g., biogas generators for R 3000). View details, including star ratings (★★★★☆).
- **Login**: Use the credentials below to test different roles.

### Testing with Credentials
1. **Farmer**:
   - **Login**: Email: `farmer@example.com`, Password: `Farmer@123`
   - **Actions**:
     - Go to **Products > Create** to add a product (e.g., “Solar Panel,” R 5000).
     - Visit **Products > Details > Biogas Generator** (`/Products/Details/3`), add a review (e.g., 4 stars, “Good system”).
     - Place an order for a product.
     - Go to **Projects > Create** to propose a project.
     - Join discussions at **Discussions**.

2. **Employee**:
   - **Login**: Email: `employee@example.com`, Password: `Employee@123`
   - **Actions**:
     - Go to **Farmers** to manage profiles.
     - Browse the marketplace at **Products**.
     - View projects and funding opportunities.

3. **Green Energy Expert**:
   - **Login**: Email: `expert@example.com`, Password: `Expert@123`
   - **Actions**:
     - Join discussions at **Discussions**.
     - View funding opportunities at **Funding**.
     - Browse the marketplace and projects.


## Troubleshooting
- **Database Errors** (e.g., “SQLite Error 19: 'UNIQUE constraint failed'”):
  - Delete `AgriEnergyConnect.db` and the `Migrations` folder, then re-run:
    ```
    dotnet ef migrations add InitialCreate --context ApplicationDbContext
    dotnet ef database update --context ApplicationDbContext
    ```
- **Build Errors**:
  - Ensure .NET 8.0 SDK is installed (`dotnet --version`).
  - Restore packages: `dotnet restore`.
- **Login Issues**:
  - Verify credentials (`farmer@example.com` / `Farmer@123`, etc.).
  - Check `SeedData.cs` for role setup.
- **404 Errors**:
  - Ensure controllers (`ProductsController`, `FarmersController`, etc.) exist.
  - Verify database is seeded (`AgriEnergyConnect.db`).

For further assistance, contact the development team or check the project repository.

## License
This project is licensed under the MIT License. See the `LICENSE` file for details.
