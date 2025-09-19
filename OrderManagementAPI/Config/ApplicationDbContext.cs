using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Models;
using OrderManagementAPI.Security;

namespace OrderManagementAPI.Config;

public class ApplicationDbContext : DbContext
{
    private readonly string? _connectionString;
    private readonly ICurrentUserService  _currentUserService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
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
        
        // Call extension method for generate Seed data
        modelBuilder.Seed();
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var userId = _currentUserService.UserId;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AuditableEntity auditable)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        auditable.CreatedDate = now;
                        auditable.CreatedBy = userId;
                        break;

                    case EntityState.Modified:
                        auditable.UpdatedDate = now;
                        auditable.UpdatedBy = userId;
                        break;
                }
            }

            if (entry is not { Entity: ISoftDeletable softDeletable, State: EntityState.Deleted }) continue;
            entry.State = EntityState.Modified; // convert to update instead of physical delete
            softDeletable.IsDeleted = true;
            softDeletable.DeletedDate = now;
            softDeletable.DeletedBy = userId;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    public DbSet<T> ExecQuery<T>() where T : class
    {
        return Set<T>();
    }
}