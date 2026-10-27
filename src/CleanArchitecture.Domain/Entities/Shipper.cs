namespace CleanArchitecture.Domain.Entities;

public class Shipper
{
    public int ShipperID { get; set; }
    public required string CompanyName { get; set; }
    public string? Phone { get; set; }
}
