namespace CleanArchitecture.Domain.Common;

public record PagedResult<T>(IEnumerable<T> Items, int Page, int PageSize, long Total)
{
    public long TotalPages => PageSize == 0 ? 0 : (long)Math.Ceiling(Total / (double)PageSize);
}
