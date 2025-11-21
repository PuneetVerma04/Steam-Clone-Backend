using Microsoft.EntityFrameworkCore;

namespace SteamClone.Backend.Entities;

/// <summary>
/// Database context for the Steam Clone application
/// Manages all entity configurations and database operations
/// </summary>
public class BackendDbContext : DbContext
{
    /// <summary>
    /// Initializes the database context with configuration options
    /// </summary>
    public BackendDbContext(DbContextOptions<BackendDbContext> options) : base(options)
    {
    }

    // DbSets represent tables in the database
    /// <summary>Table for user accounts and authentication</summary>
    public DbSet<User> Users { get; set; }

    /// <summary>Table for game catalog entries</summary>
    public DbSet<Game> Games { get; set; }

    /// <summary>Table for shopping cart items</summary>
    public DbSet<CartItem> CartItems { get; set; }

    /// <summary>Table for customer orders</summary>
    public DbSet<Order> Orders { get; set; }

    /// <summary>Table for order items</summary>
    public DbSet<OrderItem> OrderItems { get; set; }

    /// <summary>Table for game reviews</summary>
    public DbSet<Review> Reviews { get; set; }

    /// <summary>Table for promotional coupons</summary>
    public DbSet<Coupon> Coupons { get; set; }

    /// <summary>
    /// Configures entity relationships, constraints, and database schema
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity with constraints and indexes
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id); // Primary Key
            entity.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);
            entity.HasIndex(u => u.Email) // Ensure email uniqueness
                .IsUnique();
            entity.HasIndex(u => u.Username) // Ensure username uniqueness
                .IsUnique();
            entity.Property(u => u.PasswordHash)
                .HasMaxLength(500);
            entity.Property(u => u.Role)
                .HasConversion<string>() // Store enum as string in database
                .IsRequired();
            entity.Property(u => u.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()"); // Auto-set creation timestamp
            entity.Property(u => u.UpdatedAt);
        });

        // Configure Game entity with validation constraints
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(g => g.Id); // Primary Key
            entity.Property(g => g.Title)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(g => g.Description)
                .IsRequired()
                .HasMaxLength(2000);
            entity.Property(g => g.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)"); // Precision for currency
            entity.Property(g => g.Genre)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(g => g.PublisherId)
                .IsRequired();
            entity.Property(g => g.ReleaseDate)
                .IsRequired();
            entity.Property(g => g.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            // Game -> Publisher (User) relationship
            entity.HasOne(g => g.Publisher)
                .WithMany(u => u.PublishedGames)
                .HasForeignKey(g => g.PublisherId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of publishers with games
        });

        // Configure CartItem entity with composite key and relationships
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(ci => new { ci.UserId, ci.GameId }); // Composite key prevents duplicate cart entries

            entity.Property(ci => ci.Quantity)
                .IsRequired();

            // CartItem -> User relationship
            entity.HasOne(ci => ci.User)
                .WithMany(u => u.CartItems)
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Remove cart items when user is deleted

            // CartItem -> Game relationship
            entity.HasOne(ci => ci.Game)
                .WithMany(g => g.CartItems)
                .HasForeignKey(ci => ci.GameId)
                .OnDelete(DeleteBehavior.Restrict); // Remove cart item when game is deleted
        });

        // Configure Order entity with status enum conversion
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.OrderId); // Primary Key

            entity.Property(o => o.UserId)
                .IsRequired();

            entity.Property(o => o.TotalPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)"); // Precision for currency

            entity.Property(o => o.OrderDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()"); // Auto-set order timestamp

            entity.Property(o => o.Status)
                .HasConversion<string>() // Store OrderStatus enum as string
                .IsRequired()
                .HasMaxLength(50);

            // Order -> User relationship
            entity.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of users with orders
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi => oi.OrderItemId); // Primary Key

            entity.Property(oi => oi.OrderId)
                .IsRequired();

            entity.Property(oi => oi.GameId)
                .IsRequired();

            entity.Property(oi => oi.Quantity)
                .IsRequired();

            entity.Property(oi => oi.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)"); // Precision for currency

            // OrderItem -> Order relationship
            entity.HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Remove order items when order is deleted

            // OrderItem -> Game relationship
            entity.HasOne(oi => oi.Game)
                .WithMany(g => g.OrderItems)
                .HasForeignKey(oi => oi.GameId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of games that have been ordered
        });

        // Configure Reviews entity with unique constraint per user-game pair
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(r => r.ReviewId); // Primary Key

            entity.Property(r => r.UserId)
                .IsRequired();

            entity.Property(r => r.GameId)
                .IsRequired();

            entity.Property(r => r.Comment)
                .HasMaxLength(1000);

            entity.Property(r => r.Rating)
                .IsRequired();

            entity.Property(r => r.ReviewDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()"); // Auto-set review timestamp

            entity.HasIndex(r => new { r.UserId, r.GameId }) // One review per user per game
                .IsUnique();

            // Review -> User relationship
            entity.HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Remove reviews when user is deleted

            // Review -> Game relationship
            entity.HasOne(r => r.Game)
                .WithMany(g => g.Reviews)
                .HasForeignKey(r => r.GameId)
                .OnDelete(DeleteBehavior.Restrict); // Remove reviews when game is deleted
        });

        // Configure Coupon entity with unique code constraint
        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.HasKey(c => c.CouponId); // Primary Key

            entity.Property(c => c.Code)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(c => c.Code) // Ensure coupon codes are unique
                .IsUnique();

            entity.Property(c => c.CouponName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(c => c.DiscountPercent)
                .IsRequired();

            entity.Property(c => c.IsActive)
                .IsRequired()
                .HasDefaultValue(true); // New coupons are active by default

            entity.Property(c => c.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()"); // Auto-set creation timestamp

            entity.Property(c => c.ExpirationDate)
                .IsRequired();
        });
    }
}
