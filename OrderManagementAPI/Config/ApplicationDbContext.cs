using System.Reflection;
using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Config;

public class ApplicationDbContext : DbContext
{
    private readonly string? _connectionString;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public ApplicationDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    
    

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured && !string.IsNullOrEmpty(_connectionString))
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        // --- Explicit configuration for UserRole (composite key + relationships) ---
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);
        
        // Seed roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "User" },
            new Role { Id = 3, Name = "Manager" },
            new Role { Id = 4, Name = "Staff" }
        );
        
        // Optional: Seed a sample user (password hash example: "admin123")
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                Password = "$2a$12$C6UzMDM.H6dfI/f/IKcEeOaHFS0jzLjwHlQ1P2z1ZbG9vG2NnQpG6", // Use BCrypt to hash
                FullName = "Administrator",
                Email = "admin@example.com",
                Phone = "1234567890",
                IsActive = true,
                CreatedAt = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = null
            },
            new User
            {
                Id = 2,
                Username = "staff",
                Password = "$2a$12$wIuG5n5h.TmXYiL5mJfH3eG7FJ6vI8t2qX2WJxg4vQ0Zc0YxA9zGq", // Use BCrypt to hash
                FullName = "Staff",
                Email = "staff@example.com",
                Phone = "1234567890",
                IsActive = true,
                CreatedAt = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = null
            }
        );
        
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole
            {
                UserId = 1, // Admin user
                RoleId = 1,  // Admin role
                AssignedAt = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc)
            },
            new UserRole
            {
                UserId = 2, // Staff user
                RoleId = 4,  // Staff role
                AssignedAt = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc)
            }
        );
        
        // Soft delete filter
        modelBuilder.Entity<Customer>().HasQueryFilter(c => !c.IsDeleted);
        
        // Customer
        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Email)
            .IsUnique(); // optional: unique email

        // Order relationship
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict); // prevent deleting customer with orders

        // OrderItem relationship
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade); // delete items when order is deleted

        

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<T> ExecQuery<T>() where T : class
    {
        return Set<T>();
    }
}