using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface IOrderRepository
{
    Task<PagedResult<Order>> GetRows(int page, int pageSize, string? search);
    Task<Order?> GetRow(int id);
    Task<Order> Insert(Order order);
    Task<bool> Update(Order order);
    Task<bool> Delete(int[] id);
    Task<int> CountExisting(int[] id);
}
