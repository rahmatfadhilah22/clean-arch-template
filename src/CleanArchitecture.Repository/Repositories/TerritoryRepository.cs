using System.Data;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using Dapper;

namespace CleanArchitecture.Repository.Repositories;

public class TerritoryRepository(IUnitOfWork uow) : ITerritoryRepository
{
    private IDbConnection Db => uow.Connection;

    public async Task<PagedResult<Territory>> GetRows(int page, int pageSize, string? search)
    {
        const string filter = @"
            @search IS NULL
            OR t.territory_id          ILIKE '%' || @search || '%'
            OR t.territory_description ILIKE '%' || @search || '%'
            OR r.region_description    ILIKE '%' || @search || '%'";

        const string joins = @"
            FROM territories t
            LEFT JOIN region r ON r.region_id = t.region_id";

        var countSql = $"SELECT COUNT(*) {joins} WHERE {filter}";

        var pageSql = $@"
            SELECT
                    t.territory_id           AS TerritoryID
                    ,t.territory_description AS TerritoryDescription
                    ,t.region_id             AS RegionID
                    ,r.region_description    AS RegionDescription
            {joins}
            WHERE
                    {filter}
            ORDER BY
                    t.territory_id
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        var args = new
        {
            search,
            offset = (page - 1) * pageSize,
            pageSize
        };

        var total = await Db.ExecuteScalarAsync<long>(countSql, args, uow.Transaction);
        var items = await Db.QueryAsync<Territory>(pageSql, args, uow.Transaction);
        return new PagedResult<Territory>(items, page, pageSize, total);
    }

    public async Task<Territory?> GetRow(string id)
    {
        const string sql = @"
            SELECT
                    territory_id           AS TerritoryID
                    ,territory_description AS TerritoryDescription
                    ,region_id             AS RegionID
            FROM
                    territories
            WHERE
                    territory_id = @id";
        return await Db.QuerySingleOrDefaultAsync<Territory>(sql, new { id }, uow.Transaction);
    }

    public async Task<Territory> Insert(Territory territory)
    {
        const string sql = @"
            INSERT INTO territories
                    (territory_id, territory_description, region_id)
            VALUES
                    (
                        @TerritoryID
                        ,@TerritoryDescription
                        ,@RegionID
                    )
            RETURNING
                    territory_id           AS TerritoryID
                    ,territory_description AS TerritoryDescription
                    ,region_id             AS RegionID";
        return await Db.QuerySingleAsync<Territory>(sql, territory, uow.Transaction);
    }

    public async Task<bool> Update(Territory territory)
    {
        const string sql = @"
            UPDATE territories
            SET
                    territory_description = @TerritoryDescription
                    ,region_id = @RegionID
            WHERE
                    territory_id = @TerritoryID";
        return await Db.ExecuteAsync(sql, territory, uow.Transaction) > 0;
    }

    public async Task<bool> Delete(string[] id)
    {
        const string sql = @"
            DELETE FROM territories
            WHERE
                    territory_id = ANY(@Ids)";
        return await Db.ExecuteAsync(sql, new { Ids = id }, uow.Transaction) > 0;
    }

    public async Task<int> CountExisting(string[] id)
    {
        const string sql = @"
            SELECT COUNT(*)
            FROM territories
            WHERE
                    territory_id = ANY(@Ids)";
        return await Db.ExecuteScalarAsync<int>(sql, new { Ids = id }, uow.Transaction);
    }
}
