using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface IOrderService
{
    Task<PagedResult<Order>> GetRows(int page, int pageSize, string? search);
    Task<Order?> GetRow(int id);
    Task<Order> Insert(Order order);
    Task<bool> Update(int id, Order order);
    Task<bool> Delete(int[] id);
}
