using log4net;
using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Config;
using OrderManagementAPI.Dto.Request;
using OrderManagementAPI.Dto.Response;
using OrderManagementAPI.Exceptions;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Services.Impl;

public class OrderServiceImpl(ApplicationDbContext context) : IOrderService
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(OrderServiceImpl));

    public async Task<OrderResponse> CreateAsync(OrderRequest orderRequest)
    {
        Log.Debug("Creating order");
        // Prevent creating order if customer is deleted
        var customer = await context.Customers.FindAsync(orderRequest.CustomerId);
        if (customer == null || customer.IsDeleted)
        {
            Log.Error("Customer not found or deleted.");
            throw new AppException("Cannot create order for deleted customer.");
        }

        var order = new Order
        {
            CustomerId = orderRequest.CustomerId,
            Items = orderRequest.Items.Select(i => new OrderItem
            {
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        // calculate total amount
        CalculateTotals(order);

        context.Orders.Add(order);
        await context.SaveChangesAsync();
        Log.Debug("Order have been created successfully");
        return OrderResponse.ToOrderResponse(order);
    }

    public async Task<OrderResponse> UpdateAsync(int id, OrderRequest orderRequest)
    {
        Log.Debug("Updating order");
        var order = await context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            Log.Error("Order not found");
            throw new ResourceNotFoundException("Order", id);
        }

        // Replace items
        order.Items.Clear();
        foreach (var i in orderRequest.Items)
        {
            order.Items.Add(new OrderItem
            {
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            });
        }

        // Recalculate total
        CalculateTotals(order);

        await context.SaveChangesAsync();
        Log.Debug("Order have been updated successfully");
        return OrderResponse.ToOrderResponse(order);
    }

    public async Task<OrderResponse> GetByIdAsync(int id)
    {
        var order = await context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
        if (order != null) return OrderResponse.ToOrderResponse(order);
        Log.Error("Order not found");
        throw new ResourceNotFoundException("Order", id);
    }


    private static void CalculateTotals(Order order)
    {
        order.TotalAmount = order.Items.Sum(i => i.Quantity * i.UnitPrice);
    }

    public async Task<List<OrderResponse>> GetOrdersAsync(int? customerId = null, DateTime? start = null,
        DateTime? end = null)
    {
        Log.InfoFormat("Getting orders by customer= {0} startDate= {1} endDate={2}", customerId, start, end);
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

        var order = await query.ToListAsync();
        return order.Select(OrderResponse.ToOrderResponse).ToList();
    }
}