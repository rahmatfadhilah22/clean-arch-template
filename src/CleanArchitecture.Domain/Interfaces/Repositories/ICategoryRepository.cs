using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<PagedResult<Category>> GetRows(int page, int pageSize, string? search);
    Task<Category?> GetRow(int id);
    Task<Category> Insert(Category product);
    Task<bool> Update(Category product);
    Task<bool> Delete(int[] id);
    Task<int> CountExisting(int[] id);
}
