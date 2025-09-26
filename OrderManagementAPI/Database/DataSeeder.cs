using Bogus;
using OrderManagementAPI.Config;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Database;

public static class DataSeeder
{
        public static void Seed(ApplicationDbContext context, int usersCount = 10, int customersCount = 20)
    {
        if (context.Users.Any()) return; // already seeded

        // --- Roles ---
        var roles = new List<Role>
        {
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "User" },
            new Role { Id = 3, Name = "Manager" },
            new Role { Id = 4, Name = "Staff" }
        };
        context.Roles.AddRange(roles);
        context.SaveChanges();

        // --- Users ---
        var userId = 1;
        var userFaker = new Faker<User>()
            .RuleFor(u => u.Id, f => userId++)
            .RuleFor(u => u.Username, f => f.Internet.UserName())
            .RuleFor(u => u.FullName, f => f.Name.FullName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(u => u.Password, f => "AQAAAAIAAYagAAAAEJfL78Q1ms0KZAGnJQEUuU8gPUuU/JdM3raKih7WxsN+sPXxmvcp6LpeE5/7hLCm2g==") // hashed
            .RuleFor(u => u.IsActive, f => true)
            .RuleFor(u => u.CreatedAt, f => DateTime.UtcNow)
            .RuleFor(u => u.UpdatedAt, f => (DateTime?)null);

        var users = userFaker.Generate(usersCount);
        context.Users.AddRange(users);
        context.SaveChanges();

        // --- UserRoles ---
        var userRoles = new List<UserRole>();
        foreach (var user in users)
        {
            var roleId = roles[new Faker().Random.Int(0, roles.Count - 1)].Id;
            userRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = roleId,
                AssignedAt = DateTime.UtcNow
            });
        }
        context.UserRoles.AddRange(userRoles);
        context.SaveChanges();

        // --- Customers ---
        var customerId = 1;
        var customerFaker = new Faker<Customer>()
            .RuleFor(c => c.Id, f => customerId++)
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Email, f => f.Internet.Email())
            .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(c => c.IsDeleted, f => false)
            .RuleFor(c => c.CreatedDate, f => DateTime.UtcNow)
            .RuleFor(c => c.UpdatedDate, f => DateTime.UtcNow)
            .RuleFor(c => c.CreatedBy, f => users.First().Id)
            .RuleFor(c => c.UpdatedBy, f => users.First().Id);

        var customers = customerFaker.Generate(customersCount);
        context.Customers.AddRange(customers);
        context.SaveChanges();

        // --- Orders and OrderItems ---
        var orderId = 1;
        var orderItemId = 1;
        var orderFaker = new Faker<Order>()
            .RuleFor(o => o.Id, f => orderId++)
            .RuleFor(o => o.CustomerId, f => f.PickRandom(customers).Id)
            .RuleFor(o => o.OrderDate, f => f.Date.Past(1))
            .RuleFor(o => o.TotalAmount, f => f.Random.Decimal(100, 1000))
            .RuleFor(o => o.CreatedBy, f => users.First().Id)
            .RuleFor(o => o.UpdatedBy, f => users.First().Id)
            .RuleFor(o => o.CreatedDate, f => DateTime.UtcNow)
            .RuleFor(o => o.UpdatedDate, f => DateTime.UtcNow);

        var orders = orderFaker.Generate(customersCount); // 1 order per customer
        context.Orders.AddRange(orders);
        context.SaveChanges();

        var orderItemFaker = new Faker<OrderItem>()
            .RuleFor(oi => oi.Id, f => orderItemId++)
            .RuleFor(oi => oi.OrderId, f => f.PickRandom(orders).Id)
            .RuleFor(oi => oi.ProductName, f => f.Commerce.ProductName())
            .RuleFor(oi => oi.Quantity, f => f.Random.Int(1, 5))
            .RuleFor(oi => oi.UnitPrice, f => f.Random.Decimal(10, 500))
            .RuleFor(oi => oi.Subtotal, (f, oi) => oi.Quantity * oi.UnitPrice);

        var orderItems = orderItemFaker.Generate(customersCount * 2); // 2 items per customer
        context.OrderItems.AddRange(orderItems);
        context.SaveChanges();
    }

}