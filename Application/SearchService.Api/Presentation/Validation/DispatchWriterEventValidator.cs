using FluentValidation;
using SearchService.Api.Models;

namespace SearchService.Api.Presentation.Validation;

public class DispatchWriterEventValidator : AbstractValidator<DispatchWriterEvent>
{
    public DispatchWriterEventValidator()
    {
        RuleFor(d => d.Type).IsInEnum();
        RuleFor(d => d.DispatchId).NotEmpty();
        RuleFor(d => d.DispatchStatus).IsInEnum();
        RuleFor(d => d.PriceTotal).GreaterThanOrEqualTo(0);
        RuleFor(d => d.Vehicles).NotEmpty();
        RuleForEach(d => d.Vehicles).ChildRules(v => v.RuleFor(x => x.Vin).NotEmpty());
    }
}
