using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface IUsStateService
{
    Task<PagedResult<UsState>> GetRows(int page, int pageSize, string? search);
    Task<UsState?> GetRow(int id);
    Task<UsState> Insert(UsState usstate);
    Task<bool> Update(int id, UsState usstate);
    Task<bool> Delete(int[] id);
}
