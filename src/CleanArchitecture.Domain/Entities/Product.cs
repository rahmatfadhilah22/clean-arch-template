namespace CleanArchitecture.Domain.Entities;

public class Product
{
    public int ProductID { get; set; }
    public required string ProductName { get; set; }
    public int? SupplierID { get; set; }
    public int? CategoryID { get; set; }
    public string? QuantityPerUnit { get; set; }
    public float? UnitPrice { get; set; }
    public int? UnitsInStock { get; set; }
    public int? UnitsOnOrder { get; set; }
    public int? ReorderLevel { get; set; }
    public int Discontinued { get; set; }
}
