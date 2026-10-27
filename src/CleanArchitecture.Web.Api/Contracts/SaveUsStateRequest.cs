using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Web.Api.Contracts;

public record SaveUsStateRequest(
    [Range(0, short.MaxValue)] int StateID,
    [MaxLength(100)] string? StateName,
    [MaxLength(2)] string? StateAbbr,
    [MaxLength(50)] string? StateRegion);
