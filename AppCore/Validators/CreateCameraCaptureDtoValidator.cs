using FluentValidation;
using AppCore.Dto;

namespace AppCore.Validators;

public class CreateCameraCaptureDtoValidator : AbstractValidator<CreateCameraCaptureDto>
{
    public CreateCameraCaptureDtoValidator()
    {
        RuleFor(x => x.LicensePlate).NotEmpty().WithMessage("Tablica rejestracyjna jest wymagana.");
        RuleFor(x => x.Brand).NotEmpty().WithMessage("Marka pojazdu jest wymagana.");
        RuleFor(x => x.Color).NotEmpty().WithMessage("Kolor pojazdu jest wymagany.");
    }
}