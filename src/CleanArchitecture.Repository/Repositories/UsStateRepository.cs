using System.Data;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using Dapper;

namespace CleanArchitecture.Repository.Repositories;

public class UsStateRepository(IUnitOfWork uow) : IUsStateRepository
{
    private IDbConnection Db => uow.Connection;

    public async Task<PagedResult<UsState>> GetRows(int page, int pageSize, string? search)
    {
        const string filter = @"
            @search IS NULL
            OR state_name   ILIKE '%' || @search || '%'
            OR state_abbr   ILIKE '%' || @search || '%'
            OR state_region ILIKE '%' || @search || '%'";

        var countSql = $"SELECT COUNT(*) FROM us_states WHERE {filter}";

        var pageSql = $@"
            SELECT
                    state_id      AS StateID
                    ,state_name   AS StateName
                    ,state_abbr   AS StateAbbr
                    ,state_region AS StateRegion
            FROM
                    us_states
            WHERE
                    {filter}
            ORDER BY
                    state_id
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        var args = new
        {
            search,
            offset = (page - 1) * pageSize,
            pageSize
        };

        var total = await Db.ExecuteScalarAsync<long>(countSql, args, uow.Transaction);
        var items = await Db.QueryAsync<UsState>(pageSql, args, uow.Transaction);
        return new PagedResult<UsState>(items, page, pageSize, total);
    }

    public async Task<UsState?> GetRow(int id)
    {
        const string sql = @"
            SELECT
                    state_id      AS StateID
                    ,state_name   AS StateName
                    ,state_abbr   AS StateAbbr
                    ,state_region AS StateRegion
            FROM
                    us_states
            WHERE
                    state_id = @id";
        return await Db.QuerySingleOrDefaultAsync<UsState>(sql, new { id }, uow.Transaction);
    }

    // ponytail: id otomatis = MAX+1, bentrok kalau insert bersamaan; buat sequence kalau perlu.
    public async Task<UsState> Insert(UsState usstate)
    {
        const string sql = @"
            INSERT INTO us_states
                    (state_id, state_name, state_abbr, state_region)
            VALUES
                    (
                        COALESCE(NULLIF(@StateID, 0), (SELECT COALESCE(MAX(state_id), 0) + 1 FROM us_states))
                        ,@StateName
                        ,@StateAbbr
                        ,@StateRegion
                    )
            RETURNING
                    state_id      AS StateID
                    ,state_name   AS StateName
                    ,state_abbr   AS StateAbbr
                    ,state_region AS StateRegion";
        return await Db.QuerySingleAsync<UsState>(sql, usstate, uow.Transaction);
    }

    public async Task<bool> Update(UsState usstate)
    {
        const string sql = @"
            UPDATE us_states
            SET
                    state_name = @StateName
                    ,state_abbr = @StateAbbr
                    ,state_region = @StateRegion
            WHERE
                    state_id = @StateID";
        return await Db.ExecuteAsync(sql, usstate, uow.Transaction) > 0;
    }

    public async Task<bool> Delete(int[] id)
    {
        const string sql = @"
            DELETE FROM us_states
            WHERE
                    state_id = ANY(@Ids)";
        return await Db.ExecuteAsync(sql, new { Ids = id }, uow.Transaction) > 0;
    }

    public async Task<int> CountExisting(int[] id)
    {
        const string sql = @"
            SELECT COUNT(*)
            FROM us_states
            WHERE
                    state_id = ANY(@Ids)";
        return await Db.ExecuteScalarAsync<int>(sql, new { Ids = id }, uow.Transaction);
    }
}
