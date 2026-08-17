using FluentValidation;
using SearchService.Api.Models;

namespace SearchService.Api.Presentation.Validation;

public class DispatchModelValidator : AbstractValidator<DispatchModel>
{
    public DispatchModelValidator()
    {
        RuleFor(d => d.DispatchId).NotEmpty();
        RuleFor(d => d.DispatchStatus).NotEmpty();
        RuleFor(d => d.PriceTotal).GreaterThanOrEqualTo(0);
        RuleFor(d => d.Vehicle.Vin).NotEmpty();
    }
}
