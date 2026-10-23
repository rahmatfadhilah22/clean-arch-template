namespace CleanArchitecture.Domain.Entities;

public class Category
{
    public int CategoryID { get; set; }
    public required string CategoryName { get; set; }
    public string? Description { get; set; }
    public byte[]? Picture { get; set; }
}
