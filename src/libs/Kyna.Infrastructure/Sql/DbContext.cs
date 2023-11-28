using Dapper;
using Kyna.Logging;
using Npgsql;

namespace Kyna.Infrastructure.Sql;

internal class DbContext
{
    public DbContext(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public string ConnectionString { get; }

    protected NpgsqlConnection GetOpenConnection()
    {
        var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }

    protected async Task<NpgsqlConnection> GetOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }

    internal void Execute(string sql, object? parameters = null,
        int? commandTimeout = null)
    {
        if (string.IsNullOrWhiteSpace(sql)) { throw new ArgumentNullException(nameof(sql)); }

        using var connection = GetOpenConnection();
        using NpgsqlTransaction transaction = connection.BeginTransaction();

        try
        {
            connection.Execute(sql, parameters, transaction, commandTimeout);
            transaction.Commit();
        }
        catch (Exception exc)
        {
            KLogger.LogCritical(exc, $"Failure to execute SQL in {nameof(Execute)}", sql);
            transaction.Rollback();
            throw;
        }
        finally
        {
            connection.Close();
        }
    }

    internal async Task ExecuteAsync(string sql, object? parameters = null,
        int? commandTimeout = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(sql)) { throw new ArgumentNullException(nameof(sql)); }

        using var connection = await GetOpenConnectionAsync(cancellationToken);
        using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            await connection.ExecuteAsync(sql, parameters, transaction, commandTimeout);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception exc)
        {
            KLogger.LogCritical(exc, $"Failure to execute SQL in {nameof(ExecuteAsync)}", sql);
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    internal IEnumerable<T> Query<T>(string sql,
        object? parameters = null,
        int? commandTimeout = null)
    {
        if (string.IsNullOrWhiteSpace(sql)) { throw new ArgumentNullException(nameof(sql)); }

        using var connection = GetOpenConnection();

        try
        {
            return connection.Query<T>(sql, parameters, commandTimeout: commandTimeout);
        }
        catch (Exception exc)
        {
            KLogger.LogCritical(exc, $"Failure to query SQL in {nameof(Query)}", sql);
            throw;
        }
        finally
        {
            connection.Close();
        }
    }

    internal async Task<IEnumerable<T>> QueryAsync<T>(string sql,
        object? parameters = null,
        int? commandTimeout = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(sql)) { throw new ArgumentNullException(nameof(sql)); }

        using var connection = await GetOpenConnectionAsync(cancellationToken);

        try
        {
            return await connection.QueryAsync<T>(sql, parameters, commandTimeout: commandTimeout);
        }
        catch (Exception exc)
        {
            KLogger.LogCritical(exc, $"Failure to query SQL in {nameof(QueryAsync)}", sql);
            throw;
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    internal T? QueryFirstOrDefault<T>(string sql,
            object? parameters = null,
            int? commandTimeout = null)
    {
        if (string.IsNullOrWhiteSpace(sql)) { throw new ArgumentNullException(nameof(sql)); }

        using var connection = GetOpenConnection();

        try
        {
            return connection.QueryFirstOrDefault<T>(sql, parameters, commandTimeout: commandTimeout);
        }
        catch (Exception exc)
        {
            KLogger.LogCritical(exc, $"Failure to query SQL in {nameof(QueryFirstOrDefault)}", sql);
            throw;
        }
        finally
        {
            connection.Close();
        }
    }

    internal async Task<T?> QueryFirstOrDefaultAsync<T>(string sql,
        object? parameters = null,
        int? commandTimeout = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(sql)) { throw new ArgumentNullException(nameof(sql)); }

        using var connection = await GetOpenConnectionAsync(cancellationToken);

        try
        {
            return await connection.QueryFirstOrDefaultAsync<T>(sql, parameters, commandTimeout: commandTimeout);
        }
        catch (Exception exc)
        {
            KLogger.LogCritical(exc, $"Failure to query SQL in {nameof(QueryFirstOrDefaultAsync)}", sql);
            throw;
        }
        finally
        {
            await connection.CloseAsync();
        }
    }
}
