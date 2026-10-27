using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Web.Api.Contracts;

public record SaveEmployeeRequest(
    [Range(0, short.MaxValue)] int EmployeeID,
    [Required, MaxLength(20)] string LastName,
    [Required, MaxLength(10)] string FirstName,
    [MaxLength(30)] string? Title,
    [MaxLength(25)] string? TitleOfCourtesy,
    DateOnly? BirthDate,
    DateOnly? HireDate,
    [MaxLength(60)] string? Address,
    [MaxLength(15)] string? City,
    [MaxLength(15)] string? Region,
    [MaxLength(10)] string? PostalCode,
    [MaxLength(15)] string? Country,
    [MaxLength(24)] string? HomePhone,
    [MaxLength(4)] string? Extension,
    byte[]? Photo,
    string? Notes,
    int? ReportsTo,
    [MaxLength(255)] string? PhotoPath);
