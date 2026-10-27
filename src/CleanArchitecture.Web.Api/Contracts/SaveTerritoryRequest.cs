using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Web.Api.Contracts;

public record SaveTerritoryRequest(
    [Required, MaxLength(20)] string TerritoryID,
    [Required, MaxLength(60)] string TerritoryDescription,
    int RegionID);
