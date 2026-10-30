namespace CleanArchitecture.Domain.Entities;

public class Territory
{
    public required string TerritoryID { get; set; }
    public required string TerritoryDescription { get; set; }
    public int RegionID { get; set; }

    // Hanya diisi join GetRows untuk ditampilkan; tidak disimpan ke tabel territories.
    public string? RegionDescription { get; set; }
}
