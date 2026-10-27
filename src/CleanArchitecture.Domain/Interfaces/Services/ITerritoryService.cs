using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface ITerritoryService
{
    Task<PagedResult<Territory>> GetRows(int page, int pageSize, string? search);
    Task<Territory?> GetRow(string id);
    Task<Territory> Insert(Territory territory);
    Task<bool> Update(string id, Territory territory);
    Task<bool> Delete(string[] id);
}
