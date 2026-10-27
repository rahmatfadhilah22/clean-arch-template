using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface IUsStateRepository
{
    Task<PagedResult<UsState>> GetRows(int page, int pageSize, string? search);
    Task<UsState?> GetRow(int id);
    Task<UsState> Insert(UsState usstate);
    Task<bool> Update(UsState usstate);
    Task<bool> Delete(int[] id);
    Task<int> CountExisting(int[] id);
}
