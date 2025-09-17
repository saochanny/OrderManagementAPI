using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using OrderManagementAPI.Constants;
using OrderManagementAPI.Exceptions;

namespace OrderManagementAPI.Utilizes;

/// <summary>
/// Utility class for executing database operations using Dapper.
/// Supports queries, commands, and transaction handling in a generic and reusable way.
/// </summary>
public class DapperDaoUtilize(IConfiguration pvConfiguration)
{
    /// <summary>
    /// Executes a SQL query and returns the results as an enumerable of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the query result.</typeparam>
    /// <param name="query">The SQL query string.</param>
    /// <param name="parameters">Optional query parameters.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of results.</returns>
    /// <exception cref="AppException">Thrown if connection string is missing.</exception>
    public IEnumerable<T> ExecQuery<T>(string query, object? parameters = null)
    {
        var connectionString = pvConfiguration.GetConnectionString("DefaultConnection")
                               ?? throw new AppException(MessageConstant.DataSourceNotFound);
        using IDbConnection db = new SqlConnection(connectionString);

        return db.Query<T>(query, parameters);
    }


    /// <summary>
    /// Executes a SQL query within a transaction asynchronously and returns the results.
    /// Commits the transaction if successful, rolls back if any exception occurs.
    /// </summary>
    /// <typeparam name="T">The type of the query result.</typeparam>
    /// <param name="query">The SQL query string.</param>
    /// <param name="parameters">Optional query parameters.</param>
    /// <returns>A task representing the asynchronous operation returning <see cref="IEnumerable{T}"/> results.</returns>
    /// <exception cref="AppException">Thrown if connection string is missing.</exception>
    public async Task<IEnumerable<T>> ExecQueryWithTransactionAsync<T>(
        string query,
        object? parameters = null)
    {
        var connectionString = pvConfiguration.GetConnectionString("DefaultConnection")
                               ?? throw new AppException(MessageConstant.DataSourceNotFound);

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


    /// <summary>
    /// Executes a SQL command (INSERT, UPDATE, DELETE) and returns the affected row count and optionally the affected data.
    /// </summary>
    /// <typeparam name="T">The type representing the data affected by the command.</typeparam>
    /// <param name="query">The SQL command string.</param>
    /// <param name="parameters">Optional command parameters.</param>
    /// <returns>A tuple containing the number of affected rows and optionally the affected data.</returns>
    /// <exception cref="AppException">Thrown if connection string is missing.</exception>
    public (int RowsAffected, object? AffectedData) ExecCommand<T>(string query,
        object? parameters = null)
    {
        var connectionString = pvConfiguration.GetConnectionString("DefaultConnection")
                               ?? throw new AppException(MessageConstant.DataSourceNotFound);
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


    /// <summary>
    /// Executes a SQL query that returns a single integer value asynchronously.
    /// </summary>
    /// <param name="query">The SQL query string.</param>
    /// <param name="parameters">Query parameters.</param>
    /// <returns>The integer value returned by the query.</returns>
    /// <exception cref="AppException">Thrown if connection string is missing.</exception>
    public async Task<int> QueryIntValueAsync(string query, object parameters)
    {
        var connectionString = pvConfiguration.GetConnectionString("DefaultConnection")
                               ?? throw new AppException("Missing connection string");
        await using var db = new SqlConnection(connectionString);
        return await db.ExecuteScalarAsync<int>(query, parameters);
    }


    /// <summary>
    /// Executes a SQL query that returns a single string value.
    /// </summary>
    /// <param name="query">The SQL query string.</param>
    /// <param name="parameters">Optional query parameters.</param>
    /// <returns>The string value returned by the query, or null if no result is found.</returns>
    /// <exception cref="AppException">Thrown if connection string is missing.</exception>
    public string? QueryStringValue(string query, object? parameters = null)
    {
        var connectionString = pvConfiguration.GetConnectionString("DefaultConnection")
                               ?? throw new AppException(MessageConstant.DataSourceNotFound);

        using IDbConnection db = new SqlConnection(connectionString);

        // Use Dapper's QueryFirstOrDefault to get the scalar int result
        var result = db.QueryFirstOrDefault<string?>(query, parameters);

        // Return 0 if no result found
        return result ?? null;
    }


    /// <summary>
    /// Executes a custom asynchronous action within a database transaction.
    /// Commits the transaction if successful; rolls back if any exception occurs.
    /// </summary>
    /// <param name="action">A function that takes an <see cref="IDbConnection"/> and <see cref="IDbTransaction"/> and performs operations asynchronously.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="AppException">Thrown if connection string is missing.</exception>
    public async Task ExecuteInTransactionAsync(Func<IDbConnection, IDbTransaction, Task> action)
    {
        var connectionString = pvConfiguration.GetConnectionString("DefaultConnection")!;
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await using var transaction = connection.BeginTransaction();
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