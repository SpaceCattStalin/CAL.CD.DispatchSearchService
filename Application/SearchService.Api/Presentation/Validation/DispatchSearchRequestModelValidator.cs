using FluentValidation;
using SearchService.Api.Models.Search;

namespace SearchService.Api.Presentation.Validation;

public class DispatchSearchRequestModelValidator : AbstractValidator<DispatchSearchRequestModel>
{
    public DispatchSearchRequestModelValidator()
    {
        RuleFor(r => r.PriceTotalMax)
            .GreaterThanOrEqualTo(r => r.PriceTotalMin!.Value)
            .When(r => r.PriceTotalMin.HasValue && r.PriceTotalMax.HasValue)
            .WithMessage("PriceTotalMax must be greater than or equal to PriceTotalMin.");

        RuleFor(r => r.PickupDateTo)
            .GreaterThanOrEqualTo(r => r.PickupDateFrom!.Value)
            .When(r => r.PickupDateFrom.HasValue && r.PickupDateTo.HasValue)
            .WithMessage("PickupDateTo must be on or after PickupDateFrom.");

        RuleFor(r => r.DropoffDateTo)
            .GreaterThanOrEqualTo(r => r.DropoffDateFrom!.Value)
            .When(r => r.DropoffDateFrom.HasValue && r.DropoffDateTo.HasValue)
            .WithMessage("DropoffDateTo must be on or after DropoffDateFrom.");
    }
}
