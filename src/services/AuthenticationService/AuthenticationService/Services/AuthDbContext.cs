using Microsoft.EntityFrameworkCore;
using AuthenticationService.Models;

namespace AuthenticationService.Services;

/// <summary>
/// Entity Framework DbContext for authentication service
/// </summary>
public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.IsActive);
            
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);
                
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(e => e.FullName)
                .HasMaxLength(200);
                
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(500);
                
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20);
        });

        // UserRole configuration
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.Role });
            
            entity.HasOne(e => e.User)
                .WithMany(e => e.UserRoles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.Property(e => e.Role)
                .IsRequired()
                .HasMaxLength(50);
        });

        // RefreshToken configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.ExpiresAt);
            
            entity.HasOne(e => e.User)
                .WithMany(e => e.RefreshTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(e => e.ReplacedByToken)
                .HasMaxLength(500);
        });

        // ApiKey configuration
        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Prefix);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CreatedAt);
            
            entity.HasOne(e => e.User)
                .WithMany(e => e.ApiKeys)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.Property(e => e.KeyName)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.KeyHash)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(e => e.Prefix)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.Property(e => e.Scopes)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var adminUserId = Guid.Parse("550e8400-e29b-41d4-a716-446655440001");
        var testUserId = Guid.Parse("550e8400-e29b-41d4-a716-446655440002");
        var demoUserId = Guid.Parse("550e8400-e29b-41d4-a716-446655440003");

        // Seed users
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = adminUserId,
                Email = "admin@example.com",
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                FullName = "System Administrator",
                IsEmailVerified = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                ModifiedAt = DateTime.UtcNow.AddDays(-30)
            },
            new User
            {
                Id = testUserId,
                Email = "test@example.com",
                Username = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("test123"),
                FullName = "Test User",
                IsEmailVerified = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                ModifiedAt = DateTime.UtcNow.AddDays(-15)
            },
            new User
            {
                Id = demoUserId,
                Email = "demo@example.com",
                Username = "demouser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("demo123"),
                FullName = "Demo User",
                IsEmailVerified = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-7),
                ModifiedAt = DateTime.UtcNow.AddDays(-7)
            }
        );

        // Seed user roles
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { UserId = adminUserId, Role = "Admin" },
            new UserRole { UserId = adminUserId, Role = "User" },
            new UserRole { UserId = testUserId, Role = "User" },
            new UserRole { UserId = demoUserId, Role = "User" }
        );

        // Seed API keys
        modelBuilder.Entity<ApiKey>().HasData(
            new ApiKey
            {
                Id = Guid.NewGuid(),
                UserId = adminUserId,
                KeyName = "Development Key",
                KeyHash = BCrypt.Net.BCrypt.HashPassword("dev_1234567890abcdef"),
                Prefix = "dev_",
                Scopes = new[] { "read", "write" },
                ExpiresAt = DateTime.UtcNow.AddYears(1),
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new ApiKey
            {
                Id = Guid.NewGuid(),
                UserId = testUserId,
                KeyName = "Test Key",
                KeyHash = BCrypt.Net.BCrypt.HashPassword("test_abcdef1234567890"),
                Prefix = "test_",
                Scopes = new[] { "read" },
                ExpiresAt = DateTime.UtcNow.AddMonths(6),
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            }
        );
    }
}
