# Steam Clone Backend

A comprehensive ASP.NET Core Web API backend for a Steam-like game store application. This project provides a full-featured, production-ready API for managing games, users, shopping carts, orders, reviews, coupons, and analytics with extensive test coverage.

## 🚀 Features

### Core Functionality

- **User Management**: Registration, authentication, and role-based authorization (Player, Publisher, Admin)
- **Game Catalog**: Complete CRUD operations for game management with pagination, filtering by genre and price
- **Shopping Cart**: Add, remove, and manage game purchases with real-time price calculation
- **Order Processing**: Complete order management system with order history and tracking
- **Review System**: User reviews and ratings for games with validation
- **Coupon System**: Discount codes and promotional offers with expiration tracking
- **Analytics Dashboard**: Revenue tracking, game performance metrics, and sales analytics

### Security & Authentication

- JWT-based authentication with secure token management
- Role-based authorization (Player, Publisher, Admin)
- Password hashing using BCrypt
- Secure API endpoints with proper middleware
- Input validation using FluentValidation
- Exception handling middleware for consistent error responses

## 🛠 Technology Stack

- **.NET 9.0**: Latest .NET framework
- **ASP.NET Core Web API**: RESTful API framework
- **Entity Framework Core 9.0**: ORM for database operations with migrations
- **SQL Server**: Database management system
- **AutoMapper 12.0**: Object-to-object mapping
- **JWT Bearer Tokens**: Authentication mechanism
- **BCrypt.Net-Next**: Secure password hashing
- **FluentValidation**: Input validation

## 📁 Project Structure

```
SteamClone.Backend/
├── Controllers/          # API controllers
│   ├── AuthController.cs
│   ├── GamesController.cs
│   ├── UsersController.cs
│   ├── CartController.cs
│   ├── OrderController.cs
│   ├── ReviewController.cs
│   ├── CouponController.cs
│   └── AnalyticsController.cs
├── DTOs/                 # Data Transfer Objects
│   ├── Auth/
│   ├── Game/
│   ├── User/
│   ├── Cart/
│   ├── Order/
│   ├── Review/
│   ├── Coupon/
│   └── Analytics/
├── Entities/             # Domain models
│   ├── User.cs
│   ├── Game.cs
│   ├── CartItem.cs
│   ├── Order.cs
│   ├── OrderItem.cs
│   ├── Review.cs
│   ├── Coupon.cs
│   └── BackendDbContext.cs
├── Services/             # Business logic layer
│   ├── Interfaces/
│   ├── UserService.cs
│   ├── GameService.cs
│   ├── CartService.cs
│   ├── OrderService.cs
│   ├── ReviewService.cs
│   ├── CouponService.cs
│   ├── AnalyticsService.cs
│   └── JwtService.cs
├── Profiles/             # AutoMapper profiles
│   ├── CartProfile.cs
│   ├── CouponsProfile.cs
│   ├── GameProfile.cs
│   ├── OrderProfile.cs
│   └── ReviewProfile.cs
├── Validators/           # FluentValidation validators
├── Middleware/           # Custom middleware
│   └── ExceptionHandlingMiddleware.cs
├── Extensions/           # Extension methods
│   └── DbSeeder.cs
├── Settings/             # Configuration settings
├── Migrations/           # EF Core migrations
└── Properties/           # Launch settings
```

## 🔧 Installation & Setup

### Prerequisites

- .NET 9.0 SDK or later
- SQL Server (LocalDB or full version)
- Visual Studio 2022 or VS Code with C# extension
- Git (for cloning the repository)

### Steps

1. **Clone the repository**

   ```bash
   git clone https://github.com/PuneetVerma04/Game-Store-Web-App-Backend.git
   cd "Steam Clone Back"
   ```

2. **Restore packages**

   ```bash
   dotnet restore
   ```

3. **Configure database connection**
   Update the connection string in `appsettings.json` or `appsettings.Development.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SteamCloneDB;Trusted_Connection=true;"
     }
   }
   ```

4. **Configure JWT settings**
   Set up JWT configuration in `appsettings.json`:

   ```json
   {
     "JwtSettings": {
       "Key": "your-secret-key-minimum-32-characters",
       "Issuer": "SteamCloneAPI",
       "Audience": "SteamCloneClient",
       "ExpirationInMinutes": 60
     }
   }
   ```

5. **Apply database migrations**

   ```bash
   dotnet ef database update --project SteamClone.Backend
   ```

6. **Run the application**

   ```bash
   dotnet run --project SteamClone.Backend
   ```

7. **Access API documentation**
   - Development: `https://localhost:7044/swagger`
   - HTTP: `http://localhost:5062/swagger`

## 📚 API Endpoints

### Authentication

- `POST /store/auth/register` - User registration
- `POST /store/auth/login` - User login (returns JWT token)

### Games

- `GET /store/games` - Get all games (supports pagination and filtering)
  - Query params: `pageNumber`, `pageSize`, `genre`, `minPrice`, `maxPrice`
- `GET /store/games/{id}` - Get game by ID
- `POST /store/games` - Create new game (Publisher/Admin only)
- `PUT /store/games/{id}` - Update game (Publisher/Admin only)
- `DELETE /store/games/{id}` - Delete game (Admin only)

### Users

- `GET /store/users` - Get all users (Admin only)
- `GET /store/users/{id}` - Get user by ID
- `PUT /store/users/{id}` - Update user profile
- `DELETE /store/users/{id}` - Delete user (Admin only)

### Cart

- `GET /store/cart/{userId}` - Get user's cart with total price
- `POST /store/cart` - Add item to cart
- `DELETE /store/cart/{userId}/{gameId}` - Remove item from cart
- `DELETE /store/cart/{userId}` - Clear entire cart

### Orders

- `GET /store/orders` - Get user's order history
- `GET /store/orders/{id}` - Get order details with items
- `POST /store/orders` - Create new order from cart

### Reviews

- `GET /store/reviews/game/{gameId}` - Get all reviews for a game
- `GET /store/reviews/{id}` - Get specific review
- `POST /store/reviews` - Create review (requires purchase)
- `PUT /store/reviews/{id}` - Update review (own reviews only)
- `DELETE /store/reviews/{id}` - Delete review (own reviews or Admin)

### Coupons

- `GET /store/coupons` - Get all available coupons
- `GET /store/coupons/{code}` - Get coupon by code
- `POST /store/coupons` - Create coupon (Admin only)
- `POST /store/coupons/validate` - Validate coupon code
- `DELETE /store/coupons/{id}` - Delete coupon (Admin only)

### Analytics

- `GET /store/analytics/summary` - Get comprehensive analytics summary (Admin only)
- `GET /store/analytics/revenue` - Get revenue statistics
- `GET /store/analytics/games/top` - Get top performing games

## 👥 User Roles

- **Player**: Can browse games, make purchases, write reviews
- **Publisher**: Can manage their own games, view analytics for their games
- **Admin**: Full system access, user management, analytics

## 🔒 Authentication & Authorization

The API uses JWT tokens for authentication. Include the token in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

## 📦 Dependencies

### Core Packages

- **AutoMapper** (12.0.1) - Object-to-object mapping
- **AutoMapper.Extensions.Microsoft.DependencyInjection** (12.0.1) - DI integration
- **BCrypt.Net-Next** (4.0.3) - Password hashing
- **FluentValidation** (12.1.0) - Input validation framework
- **FluentValidation.AspNetCore** (11.3.1) - ASP.NET Core integration

### Authentication & Authorization

- **Microsoft.AspNetCore.Authentication.JwtBearer** (9.0.9) - JWT authentication

### Database

- **Microsoft.EntityFrameworkCore** (9.0.10) - ORM framework
- **Microsoft.EntityFrameworkCore.SqlServer** (9.0.10) - SQL Server provider
- **Microsoft.EntityFrameworkCore.Design** (9.0.10) - Design-time tools

### API Documentation

- **Microsoft.AspNetCore.OpenApi** (9.0.9) - OpenAPI support
- **Swashbuckle.AspNetCore** (9.0.6) - Swagger documentation

## 📊 Database Schema

The application uses the following main entities:

- **User**: User accounts with roles
- **Game**: Game catalog with pricing and metadata
- **CartItem**: Shopping cart items
- **Order**: Purchase orders
- **OrderItem**: Individual items in orders
- **Review**: User reviews and ratings
- **Coupon**: Discount codes

## 👨‍💻 Author

**Puneet Verma**

- GitHub: [@PuneetVerma04](https://github.com/PuneetVerma04)
- Repository: [Game-Store-Web-App-Backend](https://github.com/PuneetVerma04/Steam-Clone-Backend)
