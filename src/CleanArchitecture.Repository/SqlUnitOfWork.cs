using System.Data;
using CleanArchitecture.Domain.Interfaces;
using Npgsql;

namespace CleanArchitecture.Repository;

// Satu objek scoped = satu koneksi, dipakai semua repo dalam request yang sama
// — syarat transaksi menyambung antar repo.
public class SqlUnitOfWork(string connectionString) : IUnitOfWork
{
    public IDbConnection Connection { get; } = new NpgsqlConnection(connectionString);
    public IDbTransaction? Transaction { get; private set; }

    public Task BeginAsync()
    {
        if (Connection.State != ConnectionState.Open)
            Connection.Open();
        Transaction = Connection.BeginTransaction();
        return Task.CompletedTask;
    }

    public Task CommitAsync()
    {
        Transaction?.Commit();
        return Task.CompletedTask;
    }

    public Task RollbackAsync()
    {
        Transaction?.Rollback();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Transaction?.Dispose();
        Connection.Dispose();
    }
}
