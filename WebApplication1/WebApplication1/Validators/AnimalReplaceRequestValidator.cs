
using FluentValidation;
using WebApplication1.DTOs;

namespace WebApplication1.Validators;

public class AnimalReplaceRequestValidator : AbstractValidator<ReplaceAnimalRequest>
{
    public AnimalReplaceRequestValidator()
    {
        RuleFor(s => s.Name).MaximumLength(200).NotNull();
        RuleFor(s => s.Description).MaximumLength(200);
        RuleFor(s => s.Category).MaximumLength(200).NotNull();
        RuleFor(s => s.Area).MaximumLength(200).NotNull();
    }
}