using FluentValidation;
using Orchestify.Contracts.Boards;

namespace Orchestify.Application.Validators;

/// <summary>
/// Validator for CreateBoardRequestDto.
/// </summary>
public class CreateBoardRequestValidator : AbstractValidator<CreateBoardRequestDto>
{
    public CreateBoardRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
    }
}

/// <summary>
/// Validator for UpdateBoardRequestDto.
/// </summary>
public class UpdateBoardRequestValidator : AbstractValidator<UpdateBoardRequestDto>
{
    public UpdateBoardRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
    }
}
