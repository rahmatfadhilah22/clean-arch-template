namespace CleanArchitecture.Domain.Entities;

public class Order
{
    public int OrderID { get; set; }
    public string? CustomerID { get; set; }
    public int? EmployeeID { get; set; }
    public DateOnly? OrderDate { get; set; }
    public DateOnly? RequiredDate { get; set; }
    public DateOnly? ShippedDate { get; set; }
    public int? ShipVia { get; set; }
    public float? Freight { get; set; }
    public string? ShipName { get; set; }
    public string? ShipAddress { get; set; }
    public string? ShipCity { get; set; }
    public string? ShipRegion { get; set; }
    public string? ShipPostalCode { get; set; }
    public string? ShipCountry { get; set; }

    // Hanya diisi join GetRows untuk ditampilkan; tidak disimpan ke tabel orders.
    public string? CustomerName { get; set; }
    public string? EmployeeName { get; set; }
    public string? ShipperName { get; set; }
}
