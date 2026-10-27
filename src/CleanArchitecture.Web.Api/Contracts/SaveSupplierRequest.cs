using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Web.Api.Contracts;

public record SaveSupplierRequest(
    [Range(0, short.MaxValue)] int SupplierID,
    [Required, MaxLength(40)] string CompanyName,
    [MaxLength(30)] string? ContactName,
    [MaxLength(30)] string? ContactTitle,
    [MaxLength(60)] string? Address,
    [MaxLength(15)] string? City,
    [MaxLength(15)] string? Region,
    [MaxLength(10)] string? PostalCode,
    [MaxLength(15)] string? Country,
    [MaxLength(24)] string? Phone,
    [MaxLength(24)] string? Fax,
    string? Homepage);
