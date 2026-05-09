using AppCore.Dto;
using AppCore.Enums;
using FluentValidation;

namespace AppCore.Validators;

public class UpdateGateDtoValidator : AbstractValidator<UpdateGateDto>
{
    public UpdateGateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa bramki jest wymagana.")
            .MaximumLength(20).WithMessage("Nazwa nie może przekraczać 20 znaków.")
            .Matches(@"^[\p{L}\s\-]+$").WithMessage("Nazwa zawiera niedozwolone znaki.");
        
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Typ bramki jest wymagany.")
            .IsEnumName(typeof(GateType), caseSensitive: false)
            .WithMessage("Wprowadzony typ nie jest poprawną nazwą stałej wyliczeniowej.");
    }
}