using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Web.Api.Contracts;

public record SaveCategoryRequest(
    [Range(0, short.MaxValue)] int CategoryId,
    [Required, MaxLength(15)] string CategoryName,
    string? Description,
    byte[]? Picture = null);