using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface ITerritoryRepository
{
    Task<PagedResult<Territory>> GetRows(int page, int pageSize, string? search);
    Task<Territory?> GetRow(string id);
    Task<Territory> Insert(Territory territory);
    Task<bool> Update(Territory territory);
    Task<bool> Delete(string[] id);
    Task<int> CountExisting(string[] id);
}
