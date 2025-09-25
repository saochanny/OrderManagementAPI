using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Response;

namespace OrderManagementAPI.Infrastructure.Page;

public static class QueryableExtensions
{
    public static async Task<Page<T>> ToPageAsync<T>(this IQueryable<T> query, Pageable pageable)
    {
        var totalCount = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(pageable.SortBy))
        {
            query = query.OrderByProperty(pageable.SortBy, pageable.Ascending);
        }

        var items = await query
            .Skip(pageable.PageNumber * pageable.PageSize)
            .Take(pageable.PageSize)
            .ToListAsync();

        return new Page<T>(items, pageable.PageNumber, pageable.PageSize, totalCount);
    }
    
    private static IQueryable<T> OrderByProperty<T>(this IQueryable<T> source, string propertyName, bool ascending = true)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.PropertyOrField(parameter, propertyName);
        var lambda = Expression.Lambda(property, parameter);

        string methodName = ascending ? "OrderBy" : "OrderByDescending";

        var result = Expression.Call(
            typeof(Queryable),
            methodName,
            [typeof(T), property.Type],
            source.Expression,
            Expression.Quote(lambda));

        return source.Provider.CreateQuery<T>(result);
    }

}