using System.Reflection;
using Microsoft.EntityFrameworkCore;

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured && !string.IsNullOrEmpty(_connectionString))
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entityTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, Namespace: "OderManagementAPI.Models" });

        foreach (var entityType in entityTypes)
        {
            // If the entity has a primary key, configure it as a regular entity
            var keyProperty = entityType.GetProperties().FirstOrDefault(p => p.Name == "Id");

            if (keyProperty != null)
            {
                // Register the entity with a primary key
                var entityMethod = typeof(ModelBuilder)
                    .GetMethod("Entity", []);

                // Make sure the Entity<T> method is invoked correctly for the entity type
                var genericEntityMethod = entityMethod?.MakeGenericMethod(entityType);
                genericEntityMethod?.Invoke(modelBuilder, null);
            }
            else
            {
                // If no primary key is found, treat it as a keyless entity
                typeof(ModelBuilder)
                    .GetMethod("Entity", [])
                    ?.MakeGenericMethod(entityType);

                // Use the keyless method to register the entity
                var entityBuilder = modelBuilder.Entity(entityType);
                entityBuilder.HasNoKey();
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<T> ExecQuery<T>() where T : class
    {
        return Set<T>();
    }
}