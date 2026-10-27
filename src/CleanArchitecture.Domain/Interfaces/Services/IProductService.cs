using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface IProductService
{
    Task<PagedResult<Product>> GetRows(int page, int pageSize, string? search);
    Task<Product?> GetRow(int id);
    Task<Product> Insert(Product product);
    Task<bool> Update(int id, Product product);
    Task<bool> Delete(int[] id);
}
