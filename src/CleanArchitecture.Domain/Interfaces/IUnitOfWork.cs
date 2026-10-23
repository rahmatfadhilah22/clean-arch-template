using System.Data;

namespace CleanArchitecture.Domain.Interfaces;
public interface IUnitOfWork
{
    IDbConnection Connection { get; }
    IDbTransaction? Transaction { get; }
    Task BeginAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
