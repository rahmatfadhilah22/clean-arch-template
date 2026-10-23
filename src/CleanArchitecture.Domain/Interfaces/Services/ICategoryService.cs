using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface ICategoryService
{
    Task<PagedResult<Category>> GetRows(int page, int pageSize, string? search);
    Task<Category?> GetRow(int id);
    Task<Category> Insert(Category category);
    Task<bool> Update(int id, Category category);
    Task<bool> Delete(int[] id);
}
