using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Config;

public static class ModelBuilderExtension
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        // Seed roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "User" },
            new Role { Id = 3, Name = "Manager" },
            new Role { Id = 4, Name = "Staff" }
        );

        // Seed users (with hashed password)
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                Password = "AQAAAAIAAYagAAAAEJfL78Q1ms0KZAGnJQEUuU8gPUuU/JdM3raKih7WxsN+sPXxmvcp6LpeE5/7hLCm2g==", // BCrypt hash
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
                Password = "AQAAAAIAAYagAAAAELhQej9ijIQqMORp/JU9pT0J+6aSYM8k3f7ONdxY82W+LJFKWJIFmbegCVrSMqV7FQ==", // BCrypt hash
                FullName = "Staff",
                Email = "staff@example.com",
                Phone = "1234567891",
                IsActive = true,
                CreatedAt = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = null
            }
        );

        // Seed user roles
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole
            {
                UserId = 1, // Admin user
                RoleId = 1, // Admin role
                AssignedAt = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc)
            },
            new UserRole
            {
                UserId = 2, // Staff user
                RoleId = 4, // Staff role
                AssignedAt = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc)
            }
        );
        
        
        
        // --- Customers ---
        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                Id = 1,
                Name = "John Doe",
                Email = "john@example.com",
                Phone = "111-222-3333",
                IsDeleted = false,
                CreatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                UpdatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = 1,
                UpdatedBy = 1,
                DeletedDate = null,
                DeletedBy = null,
            },
            new Customer
            {
                Id = 2,
                Name = "Jane Smith",
                Email = "jane@example.com",
                Phone = "444-555-6666",
                IsDeleted = false,
                CreatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                UpdatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = 1,
                UpdatedBy = 1,
                DeletedDate = null,
                DeletedBy = null,
            }
        );
        
        
        
        // --- Orders ---
        modelBuilder.Entity<Order>().HasData(
            new Order
            {
                Id = 1,
                CustomerId = 1,
                OrderDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                TotalAmount = 300m,
                CreatedBy = 1,
                UpdatedBy = 1,
                CreatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                UpdatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                
            },
            new Order
            {
                Id = 2,
                CustomerId = 2,
                OrderDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                TotalAmount = 150m,
                CreatedBy = 1,
                UpdatedBy = 1,
                CreatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                UpdatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),

            }
        );
        
        
        // --- OrderItems ---
        modelBuilder.Entity<OrderItem>().HasData(
            new OrderItem
            {
                Id = 1,
                OrderId = 1,
                ProductName = "Laptop",
                Quantity = 1,
                UnitPrice = 1000m,
                Subtotal = 1000m,
            },
            new OrderItem
            {
                Id = 2,
                OrderId = 1,
                ProductName = "Mouse",
                Quantity = 2,
                UnitPrice = 50m,
                Subtotal = 100m
            },
            new OrderItem
            {
                Id = 3,
                OrderId = 2,
                ProductName = "Keyboard",
                Quantity = 3,
                UnitPrice = 50m,
                Subtotal = 150m
            }
        );
    }
}