using AppCore.Dto;
using AppCore.Enums;
using FluentValidation;

namespace AppCore.Validators;

public class ParkingGateValidator : AbstractValidator<CreateGateDto>
{
    public ParkingGateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa bramki jest wymagana.")
            .MaximumLength(20).WithMessage("Nazwa nie może przekraczać 20 znaków.")
            .Matches(@"^[\p{L}\s\-]+$").WithMessage("Nazwa zawiera niedozwolone znaki.");
        // dodaj regułę dla Location, maksymalnie 50 znaków, niedozwolonen znaki jak w Name
        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Lokalizacja bramki jest wymagana")
            .MaximumLength(50).WithMessage("Nazwa lokalizacji nie może przekraczać 50 znaków")
            .Matches(@"^[\p{L}\s\-]+$").WithMessage("Nazwa lokalizacji zawiera niedozwolone znaki.");
        
        // dodaj regułe dla Type, aby jedynymi wartościami poprawnymi były nazwy stałej wyliczeniowej GateType
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Typ bramki jest wymagany.")
            .IsEnumName(typeof(GateType), caseSensitive: false)
            .WithMessage("Wprowadzony typ nie jest poprawną nazwą stałej wyliczeniowej.");
    }
}