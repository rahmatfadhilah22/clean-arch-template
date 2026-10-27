using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface IRegionService
{
    Task<PagedResult<Region>> GetRows(int page, int pageSize, string? search);
    Task<Region?> GetRow(int id);
    Task<Region> Insert(Region region);
    Task<bool> Update(int id, Region region);
    Task<bool> Delete(int[] id);
}
