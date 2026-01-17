using FluentValidation;
using Orchestify.Contracts.Workspaces;

namespace Orchestify.Application.Validators;

/// <summary>
/// Validator for CreateWorkspaceRequestDto.
/// </summary>
public class CreateWorkspaceRequestValidator : AbstractValidator<CreateWorkspaceRequestDto>
{
    public CreateWorkspaceRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.RepositoryPath)
            .NotEmpty().WithMessage("Repository path is required.")
            .MaximumLength(500).WithMessage("Repository path must not exceed 500 characters.");

        RuleFor(x => x.DefaultBranch)
            .MaximumLength(100).WithMessage("Default branch must not exceed 100 characters.");
    }
}

/// <summary>
/// Validator for UpdateWorkspaceRequestDto.
/// </summary>
public class UpdateWorkspaceRequestValidator : AbstractValidator<UpdateWorkspaceRequestDto>
{
    public UpdateWorkspaceRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.DefaultBranch)
            .MaximumLength(100).WithMessage("Default branch must not exceed 100 characters.");
    }
}
