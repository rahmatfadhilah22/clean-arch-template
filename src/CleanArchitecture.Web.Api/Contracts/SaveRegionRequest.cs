using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Web.Api.Contracts;

public record SaveRegionRequest(
    [Range(0, short.MaxValue)] int RegionID,
    [Required, MaxLength(60)] string RegionDescription);
