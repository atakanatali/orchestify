using FluentValidation;
using Orchestify.Contracts.Tasks;

namespace Orchestify.Application.Validators;

/// <summary>
/// Validator for CreateTaskRequestDto.
/// </summary>
public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequestDto>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(500).WithMessage("Title must not exceed 500 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(5000).WithMessage("Description must not exceed 5000 characters.");

        RuleFor(x => x.Priority)
            .InclusiveBetween(0, 5).WithMessage("Priority must be between 0 and 5.");
    }
}

/// <summary>
/// Validator for UpdateTaskRequestDto.
/// </summary>
public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequestDto>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(500).WithMessage("Title must not exceed 500 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(5000).WithMessage("Description must not exceed 5000 characters.");

        RuleFor(x => x.Priority)
            .InclusiveBetween(0, 5).WithMessage("Priority must be between 0 and 5.");
    }
}
