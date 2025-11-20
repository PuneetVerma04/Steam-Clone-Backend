using SteamClone.Backend.Entities;

namespace SteamClone.Backend.Extensions;

/// <summary>
/// Utility class for seeding the database with initial test data
/// Runs on application startup to populate tables for development and testing
/// </summary>
public static class DbSeeder
{
  /// <summary>
  /// Seeds the database with sample games, users, coupons, cart items, orders, and reviews
  /// Only adds data if the respective tables are empty
  /// </summary>
  /// <param name="context">Database context to seed</param>
  public static void SeedDatabase(BackendDbContext context)
  {
    // Seed Games - populate catalog with sample games
    if (!context.Games.Any())
    {
      var games = new List<Game>
        {
            new Game
            {
                Title = "Elden Ring",
                Description = "An action RPG developed by FromSoftware.",
                Price = 59.99m,
                Genre = "RPG",
                Publisher = "Bandai Namco",
                ReleaseDate = new DateTime(2022, 2, 25),
                ImageUrl = "https://example.com/eldenring.jpg"
            },
            new Game
            {
                Title = "Cyberpunk 2077",
                Description = "Open-world RPG set in Night City.",
                Price = 39.99m,
                Genre = "Action RPG",
                Publisher = "CD Projekt",
                ReleaseDate = new DateTime(2020, 12, 10),
                ImageUrl = "https://example.com/cyberpunk.jpg"
            },
            new Game
            {
                Title = "God of War Ragnarök",
                Description = "Action-adventure featuring Kratos and Atreus.",
                Price = 69.99m,
                Genre = "Action",
                Publisher = "Sony Interactive Entertainment",
                ReleaseDate = new DateTime(2022, 11, 9),
                ImageUrl = "https://example.com/gowr.jpg"
            },
            new Game
            {
                Title = "The Witcher 3",
                Description = "Story-driven open-world RPG with Geralt.",
                Price = 29.99m,
                Genre = "RPG",
                Publisher = "CD Projekt",
                ReleaseDate = new DateTime(2015, 5, 19),
                ImageUrl = "https://example.com/witcher3.jpg"
            },
            new Game
            {
                Title = "Red Dead Redemption 2",
                Description = "Open-world western action-adventure.",
                Price = 49.99m,
                Genre = "Action Adventure",
                Publisher = "Rockstar Games",
                ReleaseDate = new DateTime(2018, 10, 26),
                ImageUrl = "https://example.com/rdr2.jpg"
            },
            new Game
            {
                Title = "Hades",
                Description = "Action roguelike dungeon crawler.",
                Price = 19.99m,
                Genre = "Roguelike",
                Publisher = "Supergiant Games",
                ReleaseDate = new DateTime(2020, 9, 17),
                ImageUrl = "https://example.com/hades.jpg"
            },
            new Game
            {
                Title = "Minecraft",
                Description = "Sandbox survival and building game.",
                Price = 26.95m,
                Genre = "Sandbox",
                Publisher = "Mojang",
                ReleaseDate = new DateTime(2011, 11, 18),
                ImageUrl = "https://example.com/minecraft.jpg"
            },
            new Game
            {
                Title = "Dark Souls III",
                Description = "Challenging action RPG in the Souls series.",
                Price = 39.99m,
                Genre = "RPG",
                Publisher = "Bandai Namco",
                ReleaseDate = new DateTime(2016, 3, 24),
                ImageUrl = "https://example.com/darksouls3.jpg"
            },
            new Game
            {
                Title = "Grand Theft Auto V",
                Description = "Open-world crime and adventure game.",
                Price = 29.99m,
                Genre = "Action Adventure",
                Publisher = "Rockstar Games",
                ReleaseDate = new DateTime(2013, 9, 17),
                ImageUrl = "https://example.com/gtav.jpg"
            },
            new Game
            {
                Title = "Hollow Knight",
                Description = "Metroidvania-style action-adventure.",
                Price = 14.99m,
                Genre = "Metroidvania",
                Publisher = "Team Cherry",
                ReleaseDate = new DateTime(2017, 2, 24),
                ImageUrl = "https://example.com/hollowknight.jpg"
            }
        };

      context.Games.AddRange(games);
      context.SaveChanges();
    }

    // Seed Users - create sample user accounts with different roles
    if (!context.Users.Any())
    {
      var users = new List<User>
    {
      new User
      {
        Username = "john_doe",
        Email = "john@example.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
        Role = UserRole.Player,
        CreatedAt = DateTime.UtcNow
      },
      new User
      {
        Username = "jane_doe",
        Email = "jane@example.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
        Role = UserRole.Player,
        CreatedAt = DateTime.UtcNow
      },
      new User
      {
        Username = "alice_smith",
        Email = "alice@example.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
        Role = UserRole.Player,
        CreatedAt = DateTime.UtcNow
      },
      new User
      {
        Username = "bob_jones",
        Email = "bob@example.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
        Role = UserRole.Publisher,
        CreatedAt = DateTime.UtcNow
      },
      new User
      {
        Username = "charlie_brown",
        Email = "charlie@example.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
        Role = UserRole.Admin,
        CreatedAt = DateTime.UtcNow
      }
    };

      context.Users.AddRange(users);
      context.SaveChanges();
    }

    // Seed Coupons - create promotional discount codes
    if (!context.Coupons.Any())
    {
      var coupons = new List<Coupons>
      {
        new Coupons
        {
          Code = "WELCOME10",
          CouponName = "Welcome 10%",
          DiscountPercent = 10,
          IsActive = true,
          CreatedAt = DateTime.UtcNow,
          ExpirationDate = DateTime.UtcNow.AddMonths(3)
        },
        new Coupons
        {
          Code = "SUMMER25",
          CouponName = "Summer Sale 25%",
          DiscountPercent = 25,
          IsActive = true,
          CreatedAt = DateTime.UtcNow,
          ExpirationDate = DateTime.UtcNow.AddMonths(1)
        }
      };

      context.Coupons.AddRange(coupons);
      context.SaveChanges();
    }

    // Seed CartItems - populate shopping carts for test users
    if (!context.CartItems.Any())
    {
      var cartItems = new List<CartItem>
      {
        new CartItem { UserId = 1, GameId = 1, Quantity = 1, Game = context.Games.First(g => g.Id == 1) },
        new CartItem { UserId = 1, GameId = 5, Quantity = 2, Game = context.Games.First(g => g.Id == 5) },
        new CartItem { UserId = 2, GameId = 3, Quantity = 1, Game = context.Games.First(g => g.Id == 3) },
        new CartItem { UserId = 3, GameId = 10, Quantity = 1, Game = context.Games.First(g => g.Id == 10) },
        new CartItem { UserId = 4, GameId = 2, Quantity = 1, Game = context.Games.First(g => g.Id == 2) }
      };

      context.CartItems.AddRange(cartItems);
      context.SaveChanges();
    }

    // Seed Orders with OrderItems - create sample purchase history
    if (!context.Orders.Any())
    {
      var orders = new List<Order>
      {
        // User 1's first order - purchased Elden Ring and RDR2
        new Order
        {
          UserId = 1,
          TotalPrice = 159.97m,
          OrderDate = DateTime.UtcNow.AddDays(-10),
          Status = OrderStatus.Completed,
          Items = new List<OrderItem>
          {
            new OrderItem { GameId = 1, Quantity = 1, Price = 59.99m }, // Elden Ring
            new OrderItem { GameId = 5, Quantity = 2, Price = 49.99m }  // RDR2
          }
        },
        // User 2's order - purchased God of War and Cyberpunk
        new Order
        {
          UserId = 2,
          TotalPrice = 109.98m,
          OrderDate = DateTime.UtcNow.AddDays(-5),
          Status = OrderStatus.Completed,
          Items = new List<OrderItem>
          {
            new OrderItem { GameId = 3, Quantity = 1, Price = 69.99m }, // God of War
            new OrderItem { GameId = 2, Quantity = 1, Price = 39.99m }  // Cyberpunk
          }
        },
        // User 3's pending order
        new Order
        {
          UserId = 3,
          TotalPrice = 44.98m,
          OrderDate = DateTime.UtcNow.AddDays(-2),
          Status = OrderStatus.Pending,
          Items = new List<OrderItem>
          {
            new OrderItem { GameId = 4, Quantity = 1, Price = 29.99m }, // Witcher 3
            new OrderItem { GameId = 10, Quantity = 1, Price = 14.99m }  // Hollow Knight
          }
        },
        // User 1's second order
        new Order
        {
          UserId = 1,
          TotalPrice = 46.94m,
          OrderDate = DateTime.UtcNow.AddDays(-15),
          Status = OrderStatus.Completed,
          Items = new List<OrderItem>
          {
            new OrderItem { GameId = 6, Quantity = 1, Price = 19.99m }, // Hades
            new OrderItem { GameId = 7, Quantity = 1, Price = 26.95m }  // Minecraft
          }
        },
        // User 2's second order - shows Elden Ring is popular
        new Order
        {
          UserId = 2,
          TotalPrice = 59.99m,
          OrderDate = DateTime.UtcNow.AddDays(-20),
          Status = OrderStatus.Completed,
          Items = new List<OrderItem>
          {
            new OrderItem { GameId = 1, Quantity = 1, Price = 59.99m }  // Elden Ring (popular!)
          }
        }
      }; context.Orders.AddRange(orders);
      context.SaveChanges();
    }

    // Seed Reviews - create sample game reviews from users
    if (!context.Reviews.Any())
    {
      var reviews = new List<Reviews>
      {
        new Reviews { UserId = 1, GameId = 1, Rating = 5, Comment = "Amazing game!", ReviewDate = DateTime.UtcNow.AddDays(-20) },
        new Reviews { UserId = 2, GameId = 1, Rating = 4, Comment = "Great but some bugs.", ReviewDate = DateTime.UtcNow.AddDays(-18) },
        new Reviews { UserId = 3, GameId = 4, Rating = 5, Comment = "Masterpiece.", ReviewDate = DateTime.UtcNow.AddDays(-30) },
        new Reviews { UserId = 4, GameId = 2, Rating = 3, Comment = "It's okay.", ReviewDate = DateTime.UtcNow.AddDays(-7) }
      };

      context.Reviews.AddRange(reviews);
      context.SaveChanges();
    }
  }
}