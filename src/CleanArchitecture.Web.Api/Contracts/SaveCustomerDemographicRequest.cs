using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Web.Api.Contracts;

public record SaveCustomerDemographicRequest(
    [Required, MaxLength(5)] string CustomerTypeID,
    string? CustomerDesc);
