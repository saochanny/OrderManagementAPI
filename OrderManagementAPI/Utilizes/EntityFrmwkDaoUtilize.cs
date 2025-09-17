using System.Linq.Expressions;
using OrderManagementAPI.Config;
using OrderManagementAPI.Constants;
using OrderManagementAPI.Exceptions;


namespace OrderManagementAPI.Utilizes;

/// <summary>
/// Utility class for executing Entity Framework queries and commands in a generic way.
/// Supports CRUD operations with optional filtering and dynamic property updates.
/// </summary>

public class EntityFrmwkDaoUtilize(IConfiguration pvConfiguration)
{
    
    
    /// <summary>
    /// Executes a query against the database with an optional query builder function.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="queryBuilder">Optional function to modify the base query.</param>
    /// <returns>An <see cref="IQueryable{T}"/> representing the query results.</returns>
    public IQueryable<T> ExecQuery<T>(Func<IQueryable<T>, IQueryable<T>>? queryBuilder = null) where T : class
    {
        var connectionString = GetConnectionString();
        var pvContext = new ApplicationDbContext(connectionString);
        IQueryable<T> query = pvContext.Set<T>();
        if (queryBuilder != null)
        {
            query = queryBuilder(query);
        }

        return query;
    }

    /// <summary>
    /// Executes a command (Insert, Update, Delete) on the database.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="entity">The entity to operate on.</param>
    /// <param name="operators">The operation type (Insert, Update, Delete).</param>
    /// <param name="filter">Optional filter to select the target entity for Update/Delete.</param>
    /// <returns>A tuple containing the number of affected rows and the affected entity.</returns>
    /// <exception cref="AppException">Thrown when operation is invalid or entity is not found.</exception>
    public (int affectedCount, T? affectedEntity) ExecCommand<T>(T entity, Operators operators,
        Expression<Func<T, bool>>? filter = null) where T : class
    {
        var connectionString = GetConnectionString();
        using var pvContext = new ApplicationDbContext(connectionString);
        return operators switch
        {
            Operators.Insert => HandleInsert(pvContext, entity),
            Operators.Update => HandleUpdate(pvContext, entity, filter),
            Operators.Delete => HandleDelete(pvContext, entity, filter),
            _ => throw new AppException(StatusCodes.Status405MethodNotAllowed,
                "Invalid operation. Use 'INSERT', 'UPDATE', or 'DELETE'.")
        };
    }

    //------------------------ Function handling operation ------------------------//
    
    #region Private Helper Methods

    /// <summary>
    /// Retrieves the database connection string from configuration.
    /// </summary>
    /// <returns>The connection string.</returns>
    /// <exception cref="AppException">Thrown if connection string is not found.</exception>
    private string GetConnectionString()
    {
        var connectionString = pvConfiguration.GetConnectionString("DefaultConnection")
                               ?? throw new AppException(StatusCodes.Status404NotFound, MessageConstant.DataSourceNotFound);
        return connectionString;
    }

    private static (int affectedCount, T? affectedEntity) HandleInsert<T>(ApplicationDbContext pvContext, T entity)
        where T : class
    {
        pvContext.Set<T>().Add(entity);
        return (pvContext.SaveChanges(), entity);
    }

    private static (int affectedCount, T? affectedEntity) HandleUpdate<T>(ApplicationDbContext pvContext, T entity,
        Expression<Func<T, bool>>? filter) where T : class
    {
        var entityToUpdate = GetEntityToUpdate(pvContext, entity, filter);

        if (entityToUpdate != null)
        {
            UpdateEntityProperties(pvContext, entityToUpdate, entity);
            return (pvContext.SaveChanges(), entityToUpdate);
        }

        throw new AppException(StatusCodes.Status404NotFound, "Entity not found for update.");
    }

    private static T? GetEntityToUpdate<T>(ApplicationDbContext pvContext, T entity, Expression<Func<T, bool>>? filter)
        where T : class
    {
        if (filter != null)
        {
            return pvContext.Set<T>().FirstOrDefault(filter);
        }
        else
        {
            return pvContext.Set<T>().Find(entity.GetType().GetProperty("Id")?.GetValue(entity));
        }
    }

    private static void UpdateEntityProperties<T>(ApplicationDbContext pvContext, T entityToUpdate, T entity)
        where T : class
    {
        var primaryKeyProperty = pvContext.Model.FindEntityType(typeof(T))?
            .FindPrimaryKey()
            ?.Properties
            .FirstOrDefault()?.Name;

        var entry = pvContext.Entry(entityToUpdate);

        foreach (var property in entity.GetType().GetProperties())
        {
            if (property.Name == primaryKeyProperty)
                continue;

            var newValue = property.GetValue(entity);
            var existingValue = property.GetValue(entityToUpdate);

            if (newValue != null && !newValue.Equals(existingValue))
            {
                entry.Property(property.Name).CurrentValue = newValue;
                entry.Property(property.Name).IsModified = true;
            }
            else
            {
                entry.Property(property.Name).IsModified = false;
            }
        }
    }

    private static (int affectedCount, T? affectedEntity) HandleDelete<T>(ApplicationDbContext pvContext, T entity,
        Expression<Func<T, bool>>? filter) where T : class
    {
        if (filter != null)
        {
            var entityToDelete = pvContext.Set<T>().FirstOrDefault(filter);
            if (entityToDelete != null)
            {
                pvContext.Set<T>().Remove(entityToDelete);
            }
            else
            {
                throw new AppException(StatusCodes.Status404NotFound,
                    "Entity not found for deletion with the given filter.");
            }
        }
        else
        {
            pvContext.Set<T>().Remove(entity);
        }

        return (pvContext.SaveChanges(), entity);
    }
    #endregion
}