using System.Data;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using Dapper;

namespace CleanArchitecture.Repository.Repositories;

public class RegionRepository(IUnitOfWork uow) : IRegionRepository
{
    private IDbConnection Db => uow.Connection;

    public async Task<PagedResult<Region>> GetRows(int page, int pageSize, string? search)
    {
        const string filter = @"
            @search IS NULL
            OR region_description ILIKE '%' || @search || '%'";

        var countSql = $"SELECT COUNT(*) FROM region WHERE {filter}";

        var pageSql = $@"
            SELECT
                    region_id           AS RegionID
                    ,region_description AS RegionDescription
            FROM
                    region
            WHERE
                    {filter}
            ORDER BY
                    region_id
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        var args = new
        {
            search,
            offset = (page - 1) * pageSize,
            pageSize
        };

        var total = await Db.ExecuteScalarAsync<long>(countSql, args, uow.Transaction);
        var items = await Db.QueryAsync<Region>(pageSql, args, uow.Transaction);
        return new PagedResult<Region>(items, page, pageSize, total);
    }

    public async Task<Region?> GetRow(int id)
    {
        const string sql = @"
            SELECT
                    region_id           AS RegionID
                    ,region_description AS RegionDescription
            FROM
                    region
            WHERE
                    region_id = @id";
        return await Db.QuerySingleOrDefaultAsync<Region>(sql, new { id }, uow.Transaction);
    }

    // ponytail: id otomatis = MAX+1, bentrok kalau insert bersamaan; buat sequence kalau perlu.
    public async Task<Region> Insert(Region region)
    {
        const string sql = @"
            INSERT INTO region
                    (region_id, region_description)
            VALUES
                    (
                        COALESCE(NULLIF(@RegionID, 0), (SELECT COALESCE(MAX(region_id), 0) + 1 FROM region))
                        ,@RegionDescription
                    )
            RETURNING
                    region_id           AS RegionID
                    ,region_description AS RegionDescription";
        return await Db.QuerySingleAsync<Region>(sql, region, uow.Transaction);
    }

    public async Task<bool> Update(Region region)
    {
        const string sql = @"
            UPDATE region
            SET
                    region_description = @RegionDescription
            WHERE
                    region_id = @RegionID";
        return await Db.ExecuteAsync(sql, region, uow.Transaction) > 0;
    }

    public async Task<bool> Delete(int[] id)
    {
        const string sql = @"
            DELETE FROM region
            WHERE
                    region_id = ANY(@Ids)";
        return await Db.ExecuteAsync(sql, new { Ids = id }, uow.Transaction) > 0;
    }

    public async Task<int> CountExisting(int[] id)
    {
        const string sql = @"
            SELECT COUNT(*)
            FROM region
            WHERE
                    region_id = ANY(@Ids)";
        return await Db.ExecuteScalarAsync<int>(sql, new { Ids = id }, uow.Transaction);
    }
}
