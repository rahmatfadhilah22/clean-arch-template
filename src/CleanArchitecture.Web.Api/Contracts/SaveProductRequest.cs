using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Web.Api.Contracts;

public record SaveProductRequest(
    [Range(0, short.MaxValue)] int ProductID,
    [Required, MaxLength(40)] string ProductName,
    int? SupplierID,
    int? CategoryID,
    [MaxLength(20)] string? QuantityPerUnit,
    float? UnitPrice,
    int? UnitsInStock,
    int? UnitsOnOrder,
    int? ReorderLevel,
    int Discontinued);
