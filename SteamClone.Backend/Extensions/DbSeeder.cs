using SteamClone.Backend.Entities;

namespace SteamClone.Backend.Extensions;

public static class DbSeeder
{
  public static void SeedDatabase(BackendDbContext context)
  {
    // Seed Games
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

    // Seed Users
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
  }
}