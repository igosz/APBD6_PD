
using FluentValidation;
using WebApplication1.DTOs;

namespace WebApplication1.Validators;



public class AnimalCreateRequestValidator : AbstractValidator<CreateAnimalRequest>
{
    public AnimalCreateRequestValidator()
    {
        RuleFor(s => s.Name).MaximumLength(200).NotNull();
        RuleFor(s => s.Description).MaximumLength(200);
        RuleFor(s => s.Category).MaximumLength(200).NotNull();
        RuleFor(s => s.Area).MaximumLength(200).NotNull();
    }
}