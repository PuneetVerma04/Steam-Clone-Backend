using Microsoft.EntityFrameworkCore;

namespace SteamClone.Backend.Entities;

public class BackendDbContext : DbContext
{
    public BackendDbContext(DbContextOptions<BackendDbContext> options) : base(options)
    {
    }

    // DbSets for all entities
    public DbSet<User> Users { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Reviews> Reviews { get; set; }
    public DbSet<Coupons> Coupons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id); // Primary Key
            entity.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);
            entity.HasIndex(u => u.Email) // Unique index on Email
                .IsUnique();
            entity.HasIndex(u => u.Username) // Unique index on Username
                .IsUnique();
            entity.Property(u => u.PasswordHash)
                .HasMaxLength(500);
            entity.Property(u => u.Role)
                .HasConversion<string>()
                .IsRequired();
            entity.Property(u => u.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()"); // Default to current UTC time
            entity.Property(u => u.UpdatedAt);
        });

        // Configure Game entity
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
                .HasColumnType("decimal(18,2)");
            entity.Property(g => g.Genre)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(g => g.Publisher)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(g => g.ReleaseDate)
                .IsRequired();
            entity.Property(g => g.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);
        });

        // Configure CartItem entity (for shopping cart)
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(ci => new { ci.UserId, ci.GameId }); // Composite Primary Key

            entity.Property(ci => ci.Quantity)
                .IsRequired();

            entity.HasOne(ci => ci.Game) // Configure relationship with Game
                .WithMany()
                .HasForeignKey(ci => ci.GameId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete when Game is deleted
        });

        // Configure Order entity
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.OrderId); // Primary Key

            entity.Property(o => o.UserId)
                .IsRequired();

            entity.Property(o => o.TotalPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)"); // Price with 2 decimal places

            entity.Property(o => o.OrderDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()"); // Default to current UTC time

            entity.Property(o => o.Status)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(50);

            // Store order items as JSON or owned entities
            entity.Ignore(o => o.Items);
        });

        // Configure Reviews entity
        modelBuilder.Entity<Reviews>(entity =>
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
                .HasDefaultValueSql("GETUTCDATE()"); // Default to current UTC time

            entity.HasIndex(r => new { r.UserId, r.GameId }) // Prevent duplicate reviews
                .IsUnique();
        });

        // Configure Coupons entity
        modelBuilder.Entity<Coupons>(entity =>
        {
            entity.HasKey(c => c.CouponId); // Primary Key

            entity.Property(c => c.Code)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(c => c.Code) // Unique index on Code
                .IsUnique();

            entity.Property(c => c.CouponName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(c => c.DiscountPercent)
                .IsRequired();

            entity.Property(c => c.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(c => c.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()"); // Default to current UTC time

            entity.Property(c => c.ExpirationDate)
                .IsRequired();
        });
    }
}
