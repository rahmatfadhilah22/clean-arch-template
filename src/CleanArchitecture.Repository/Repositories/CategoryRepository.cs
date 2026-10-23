using System.Data;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using Dapper;

namespace CleanArchitecture.Repository.Repositories;

public class CategoryRepository(IUnitOfWork uow) : ICategoryRepository
{
    private IDbConnection Db => uow.Connection;

    public async Task<PagedResult<Category>> GetRows(int page, int pageSize, string? search)
    {
        const string filter = @"
            @search IS NULL
            OR category_name ILIKE '%' || @search || '%'
            OR description   ILIKE '%' || @search || '%'";

        var countSql = $"SELECT COUNT(*) FROM categories WHERE {filter}";

        var pageSql = $@"
            SELECT
                    category_id   AS CategoryID
                    ,category_name AS CategoryName
                    ,description   AS Description
            FROM
                    categories
            WHERE
                    {filter}
            ORDER BY
                    category_id
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        var args = new
        {
            search,
            offset = (page - 1) * pageSize,
            pageSize
        };

        var total = await Db.ExecuteScalarAsync<long>(countSql, args, uow.Transaction);
        var items = await Db.QueryAsync<Category>(pageSql, args, uow.Transaction);
        return new PagedResult<Category>(items, page, pageSize, total);
    }

    public async Task<Category?> GetRow(int id)
    {
        const string sql = @"
            SELECT
                    category_id   AS CategoryID
                    ,category_name AS CategoryName
                    ,description   AS Description
                    ,picture       AS Picture
            FROM
                    categories
            WHERE
                    category_id = @id";
        return await Db.QuerySingleOrDefaultAsync<Category>(sql, new { id }, uow.Transaction);
    }

    public async Task<Category> Insert(Category category)
    {
        const string sql = @"
            INSERT INTO categories
                    (category_id, category_name, description, picture)
            VALUES
                    (
                        COALESCE(NULLIF(@CategoryID, 0), nextval(pg_get_serial_sequence('categories', 'category_id')))
                        ,@CategoryName
                        ,@Description
                        ,@Picture
                    )
            RETURNING
                    category_id   AS CategoryID
                    ,category_name AS CategoryName
                    ,description   AS Description
                    ,picture       AS Picture";
        return await Db.QuerySingleAsync<Category>(sql, category, uow.Transaction);
    }

    public async Task<bool> Update(Category category)
    {
        const string sql = @"
            UPDATE categories
            SET
                    category_name = @CategoryName
                    ,description  = @Description
                    ,picture      = @Picture
            WHERE
                    category_id = @CategoryID";
        return await Db.ExecuteAsync(sql, category, uow.Transaction) > 0;
    }

    public async Task<bool> Delete(int[] id)
    {
        const string sql = @"
            DELETE FROM categories
            WHERE
                    category_id = ANY(@Ids)";
        return await Db.ExecuteAsync(sql, new { Ids = id }, uow.Transaction) > 0;
    }

    public async Task<int> CountExisting(int[] id)
    {
        const string sql = @"
            SELECT COUNT(*)
            FROM categories
            WHERE
                    category_id = ANY(@Ids)";
        return await Db.ExecuteScalarAsync<int>(sql, new { Ids = id }, uow.Transaction);
    }
}
