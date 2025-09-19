using log4net;
using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Config;
using OrderManagementAPI.Dto;
using OrderManagementAPI.Dto.Request;
using OrderManagementAPI.Dto.Response;
using OrderManagementAPI.Exceptions;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Services.Impl;

public class CustomerServiceImpl(ApplicationDbContext context) : ICustomerService
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(CustomerServiceImpl));

    public async Task<CustomerResponse> CreateAsync(CustomerRequest customerRequest)
    {
        var customer = new Customer
        {
            Name = customerRequest.Name,
            Email = customerRequest.Email,
            Phone = customerRequest.Phone,
        };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();
        Log.Info("Customer is created");
        return CustomerResponse.ToCustomerResponse(customer);
    }

    public async Task<List<CustomerResponse>> GetAllAsync()
    {
        Log.Info("Get all customers");
        var customers = await context.Customers.ToListAsync();
        return customers.Select(CustomerResponse.ToCustomerResponse).ToList();
    }


    public async Task<CustomerResponse> GetByIdAsync(int id)
    {
        Log.InfoFormat("Get Customer by id {0}", id);
        var customer = await GetById(id);
        return CustomerResponse.ToCustomerResponse(customer);
    }


    private async Task<Customer> GetById(int id)
    {
        return await context.Customers.FindAsync(id) ?? throw new ResourceNotFoundException("Customer", id);
    }

    public async Task<CustomerResponse> UpdateAsync(int id, CustomerRequest updateRequest)
    {
        Log.InfoFormat("Update Customer by id {0}", id);
        var customer = await GetById(id);
        PrepareCustomer(customer, updateRequest);
        context.Customers.Update(customer);
        await context.SaveChangesAsync();
        return CustomerResponse.ToCustomerResponse(customer);
    }

    private static void PrepareCustomer(Customer customer, CustomerRequest updateRequest)
    {
        customer.Email = updateRequest.Email;
        customer.Phone = updateRequest.Phone;
        customer.Name = updateRequest.Name;
    }

    public async Task SoftDeleteAsync(int id)
    {
        Log.InfoFormat("Soft Delete Customer by id {0}", id);
        var customer = await context.Customers.FindAsync(id);
        if (customer == null) return;

        context.Customers.Remove(customer); // mark for delete with custom auditable
        await context.SaveChangesAsync(); // EF will convert to soft delete automatically
        Log.Info("Customer is deleted");
    }
}