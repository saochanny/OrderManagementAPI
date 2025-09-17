using System.Data;
using facescan.Constants;
using facescan.Exceptions;
using Dapper;
using Microsoft.Data.SqlClient;

namespace facescan.Utilizes;

public class DapperDaoUtilize(IConfiguration pvConfiguration)
{
    public IEnumerable<T> ExecQuery<T>(string query, object? parameters = null)
    {
        var connectionString = pvConfiguration.GetConnectionString("DefaultConnection")
                               ?? throw new AppException(Strings.DataSourceNotFound);
        using IDbConnection db = new SqlConnection(connectionString);

        return db.Query<T>(query, parameters);
    }


    public async Task<IEnumerable<T>> ExecQueryWithTransactionAsync<T>(
     string query,
     object? parameters = null)
    {
        var connectionString = pvConfiguration.GetConnectionString("DefaultConnection")
                               ?? throw new AppException(Strings.DataSourceNotFound);

        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();

        await using var tx = await conn.BeginTransactionAsync();
        try
        {
            var result = await conn.QueryAsync<T>(query, parameters, tx);
            await tx.CommitAsync();
            return result;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public (int RowsAffected, object? AffectedData) ExecCommand<T>(string query,
        object? parameters = null)
    {
        var connectionString = pvConfiguration.GetConnectionString("DefaultConnection")
                               ?? throw new AppException(Strings.DataSourceNotFound);
        using IDbConnection db = new SqlConnection(connectionString);
        // Execute command and get affected row count
        var rowsAffected = db.Execute(query, parameters);
        // Fetch the affected data if it is an INSERT or UPDATE operation
        object? affectedData = null;
        if (typeof(T) != typeof(object) && rowsAffected > 0)
        {
            affectedData = parameters; // Return input parameters as the affected data
        }

        return (rowsAffected, affectedData);
    }

   public async Task<int> QueryIntValueAsync(string query, object parameters)
{
    var connectionString = pvConfiguration.GetConnectionString("DefaultConnection")
                           ?? throw new AppException("Missing connection string");
    using var db = new SqlConnection(connectionString);
    return await db.ExecuteScalarAsync<int>(query, parameters);
}

    public string QueryStringValue(string query, object? parameters = null)
    {
        var connectionString = pvConfiguration.GetConnectionString("DefaultConnection")
                               ?? throw new AppException(Strings.DataSourceNotFound);

        using IDbConnection db = new SqlConnection(connectionString);

        // Use Dapper's QueryFirstOrDefault to get the scalar int result
        var result = db.QueryFirstOrDefault<string?>(query, parameters);

        // Return 0 if no result found
        return result ?? null;
    }


    public async Task ExecuteInTransactionAsync(Func<IDbConnection, IDbTransaction, Task> action)
    {
        var connectionString = pvConfiguration.GetConnectionString("DefaultConnection")!;
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();
        try
        {
            await action(connection, transaction);
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw; // Optional: log or rethrow
        }
    }
}
