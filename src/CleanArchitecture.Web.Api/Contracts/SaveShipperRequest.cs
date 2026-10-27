using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Web.Api.Contracts;

public record SaveShipperRequest(
    [Range(0, short.MaxValue)] int ShipperID,
    [Required, MaxLength(40)] string CompanyName,
    [MaxLength(24)] string? Phone);
