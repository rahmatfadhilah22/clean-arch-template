using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Web.Api.Contracts;

public record SaveOrderRequest(
    [Range(0, short.MaxValue)] int OrderID,
    [MaxLength(5)] string? CustomerID,
    int? EmployeeID,
    DateOnly? OrderDate,
    DateOnly? RequiredDate,
    DateOnly? ShippedDate,
    int? ShipVia,
    float? Freight,
    [MaxLength(40)] string? ShipName,
    [MaxLength(60)] string? ShipAddress,
    [MaxLength(15)] string? ShipCity,
    [MaxLength(15)] string? ShipRegion,
    [MaxLength(10)] string? ShipPostalCode,
    [MaxLength(15)] string? ShipCountry);
