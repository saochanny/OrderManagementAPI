using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Config;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Services.Impl;

public class CustomerServiceImpl(ApplicationDbContext context)
{
    public async Task<Customer> CreateAsync(Customer customer)
    {
        context.Customers.Add(customer);
        await context.SaveChangesAsync();
        return customer;
    }

    public async Task<List<Customer>> GetAllAsync() =>
        await context.Customers.ToListAsync();

    public async Task<Customer?> GetByIdAsync(int id) =>
        await context.Customers.FindAsync(id);

    public async Task<Customer?> UpdateAsync(Customer customer)
    {
        context.Customers.Update(customer);
        await context.SaveChangesAsync();
        return customer;
    }

    public async Task SoftDeleteAsync(int id)
    {
        var customer = await context.Customers.FindAsync(id);
        if (customer == null) return;
        customer.IsDeleted = true;
        await context.SaveChangesAsync();
    }
}