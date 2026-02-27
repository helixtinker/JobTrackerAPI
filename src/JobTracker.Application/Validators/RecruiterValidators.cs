using FluentValidation;
using JobTracker.Application.Dtos;

namespace JobTracker.Application.Validators;

public class CreateRecruiterDtoValidator : AbstractValidator<CreateRecruiterDto>
{
    public CreateRecruiterDtoValidator()
    {
        RuleFor(x => x.RecruiterName)
            .NotEmpty().WithMessage("Recruiter name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Company)
            .MaximumLength(200)
            .When(x => x.Company != null);

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .MaximumLength(255)
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .When(x => x.Phone != null);

        RuleFor(x => x.LinkedInUrl)
            .MaximumLength(500)
            .When(x => x.LinkedInUrl != null);

        RuleFor(x => x.RecruiterStatusId)
            .GreaterThan(0).WithMessage("'Recruiter Status Id' must be a positive number.")
            .When(x => x.RecruiterStatusId.HasValue);
    }
}

public class UpdateRecruiterDtoValidator : AbstractValidator<UpdateRecruiterDto>
{
    public UpdateRecruiterDtoValidator()
    {
        RuleFor(x => x.RecruiterName)
            .NotEmpty().WithMessage("Recruiter name cannot be set to an empty string.")
            .MaximumLength(200)
            .When(x => x.RecruiterName != null);

        RuleFor(x => x.Company)
            .MaximumLength(200)
            .When(x => x.Company != null);

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .MaximumLength(255)
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .When(x => x.Phone != null);

        RuleFor(x => x.LinkedInUrl)
            .MaximumLength(500)
            .When(x => x.LinkedInUrl != null);

        RuleFor(x => x.RecruiterStatusId)
            .GreaterThan(0).WithMessage("'Recruiter Status Id' must be a positive number.")
            .When(x => x.RecruiterStatusId.HasValue);
    }
}
