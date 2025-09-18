using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Config;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Services.Impl;

public class OrderServiceImpl(ApplicationDbContext context)
{

    public async Task<Order> CreateAsync(Order order)
    {
        // Prevent creating order if customer is deleted
        var customer = await context.Customers
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == order.CustomerId);
        if (customer == null || customer.IsDeleted)
            throw new InvalidOperationException("Cannot create order for deleted customer.");

        CalculateTotals(order);

        context.Orders.Add(order);
        await context.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> UpdateAsync(Order order)
    {
        CalculateTotals(order);
        context.Orders.Update(order);
        await context.SaveChangesAsync();
        return order;
    }

    private void CalculateTotals(Order order)
    {
        foreach (var item in order.Items)
        {
            item.Subtotal = item.Quantity * item.UnitPrice;
        }
        order.TotalAmount = order.Items.Sum(i => i.Subtotal);
    }

    public async Task<List<Order>> GetOrdersAsync(int? customerId = null, DateTime? start = null, DateTime? end = null)
    {
        var query = context.Orders
            .Include(o => o.Items)
            .Include(o => o.Customer)
            .AsQueryable();

        if (customerId.HasValue)
            query = query.Where(o => o.CustomerId == customerId.Value);

        if (start.HasValue)
            query = query.Where(o => o.OrderDate >= start.Value);

        if (end.HasValue)
            query = query.Where(o => o.OrderDate <= end.Value);

        return await query.ToListAsync();
    }
}
