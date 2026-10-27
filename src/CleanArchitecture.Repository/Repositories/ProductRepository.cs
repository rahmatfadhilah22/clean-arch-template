using System.Data;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using Dapper;

namespace CleanArchitecture.Repository.Repositories;

public class ProductRepository(IUnitOfWork uow) : IProductRepository
{
    private IDbConnection Db => uow.Connection;

    public async Task<PagedResult<Product>> GetRows(int page, int pageSize, string? search)
    {
        const string filter = @"
            @search IS NULL
            OR product_name      ILIKE '%' || @search || '%'
            OR quantity_per_unit ILIKE '%' || @search || '%'";

        var countSql = $"SELECT COUNT(*) FROM products WHERE {filter}";

        var pageSql = $@"
            SELECT
                    product_id         AS ProductID
                    ,product_name      AS ProductName
                    ,supplier_id       AS SupplierID
                    ,category_id       AS CategoryID
                    ,quantity_per_unit AS QuantityPerUnit
                    ,unit_price        AS UnitPrice
                    ,units_in_stock    AS UnitsInStock
                    ,units_on_order    AS UnitsOnOrder
                    ,reorder_level     AS ReorderLevel
                    ,discontinued      AS Discontinued
            FROM
                    products
            WHERE
                    {filter}
            ORDER BY
                    product_id
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        var args = new
        {
            search,
            offset = (page - 1) * pageSize,
            pageSize
        };

        var total = await Db.ExecuteScalarAsync<long>(countSql, args, uow.Transaction);
        var items = await Db.QueryAsync<Product>(pageSql, args, uow.Transaction);
        return new PagedResult<Product>(items, page, pageSize, total);
    }

    public async Task<Product?> GetRow(int id)
    {
        const string sql = @"
            SELECT
                    product_id         AS ProductID
                    ,product_name      AS ProductName
                    ,supplier_id       AS SupplierID
                    ,category_id       AS CategoryID
                    ,quantity_per_unit AS QuantityPerUnit
                    ,unit_price        AS UnitPrice
                    ,units_in_stock    AS UnitsInStock
                    ,units_on_order    AS UnitsOnOrder
                    ,reorder_level     AS ReorderLevel
                    ,discontinued      AS Discontinued
            FROM
                    products
            WHERE
                    product_id = @id";
        return await Db.QuerySingleOrDefaultAsync<Product>(sql, new { id }, uow.Transaction);
    }

    // ponytail: id otomatis = MAX+1, bentrok kalau insert bersamaan; buat sequence kalau perlu.
    public async Task<Product> Insert(Product product)
    {
        const string sql = @"
            INSERT INTO products
                    (product_id, product_name, supplier_id, category_id, quantity_per_unit, unit_price, units_in_stock, units_on_order, reorder_level, discontinued)
            VALUES
                    (
                        COALESCE(NULLIF(@ProductID, 0), (SELECT COALESCE(MAX(product_id), 0) + 1 FROM products))
                        ,@ProductName
                        ,@SupplierID
                        ,@CategoryID
                        ,@QuantityPerUnit
                        ,@UnitPrice
                        ,@UnitsInStock
                        ,@UnitsOnOrder
                        ,@ReorderLevel
                        ,@Discontinued
                    )
            RETURNING
                    product_id         AS ProductID
                    ,product_name      AS ProductName
                    ,supplier_id       AS SupplierID
                    ,category_id       AS CategoryID
                    ,quantity_per_unit AS QuantityPerUnit
                    ,unit_price        AS UnitPrice
                    ,units_in_stock    AS UnitsInStock
                    ,units_on_order    AS UnitsOnOrder
                    ,reorder_level     AS ReorderLevel
                    ,discontinued      AS Discontinued";
        return await Db.QuerySingleAsync<Product>(sql, product, uow.Transaction);
    }

    public async Task<bool> Update(Product product)
    {
        const string sql = @"
            UPDATE products
            SET
                    product_name = @ProductName
                    ,supplier_id = @SupplierID
                    ,category_id = @CategoryID
                    ,quantity_per_unit = @QuantityPerUnit
                    ,unit_price = @UnitPrice
                    ,units_in_stock = @UnitsInStock
                    ,units_on_order = @UnitsOnOrder
                    ,reorder_level = @ReorderLevel
                    ,discontinued = @Discontinued
            WHERE
                    product_id = @ProductID";
        return await Db.ExecuteAsync(sql, product, uow.Transaction) > 0;
    }

    public async Task<bool> Delete(int[] id)
    {
        const string sql = @"
            DELETE FROM products
            WHERE
                    product_id = ANY(@Ids)";
        return await Db.ExecuteAsync(sql, new { Ids = id }, uow.Transaction) > 0;
    }

    public async Task<int> CountExisting(int[] id)
    {
        const string sql = @"
            SELECT COUNT(*)
            FROM products
            WHERE
                    product_id = ANY(@Ids)";
        return await Db.ExecuteScalarAsync<int>(sql, new { Ids = id }, uow.Transaction);
    }
}
