using FluentValidation;
using JobTracker.Application.Dtos;

namespace JobTracker.Application.Validators;

public class CreateJobApplicationDtoValidator : AbstractValidator<CreateJobApplicationDto>
{
    public CreateJobApplicationDtoValidator()
    {
        RuleFor(x => x.CompanyName)
            .MaximumLength(200);

        RuleFor(x => x.JobTitle)
            .MaximumLength(200);

        RuleFor(x => x.Location)
            .MaximumLength(200);

        RuleFor(x => x.JobPostUrl)
            .MaximumLength(1000);

        RuleFor(x => x.CompanyWebsite)
            .MaximumLength(1000)
            .Must(BeAValidUrl).WithMessage("'{PropertyName}' must be a valid HTTP or HTTPS URL.")
            .When(x => !string.IsNullOrEmpty(x.CompanyWebsite));

        RuleFor(x => x.TechFocus)
            .MaximumLength(500);

        RuleFor(x => x.StatusId)
            .GreaterThan(0).WithMessage("'Status Id' must be a positive number.")
            .When(x => x.StatusId.HasValue);

        RuleFor(x => x.RecruiterId)
            .GreaterThan(0).WithMessage("'Recruiter Id' must be a positive number.")
            .When(x => x.RecruiterId.HasValue);
    }

    private static bool BeAValidUrl(string? url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}

public class UpdateJobApplicationDtoValidator : AbstractValidator<UpdateJobApplicationDto>
{
    public UpdateJobApplicationDtoValidator()
    {
        RuleFor(x => x.CompanyName)
            .MaximumLength(200)
            .When(x => x.CompanyName != null);

        RuleFor(x => x.JobTitle)
            .MaximumLength(200)
            .When(x => x.JobTitle != null);

        RuleFor(x => x.Location)
            .MaximumLength(200)
            .When(x => x.Location != null);

        RuleFor(x => x.JobPostUrl)
            .MaximumLength(1000);

        RuleFor(x => x.CompanyWebsite)
            .MaximumLength(1000)
            .Must(BeAValidUrl).WithMessage("'{PropertyName}' must be a valid HTTP or HTTPS URL.")
            .When(x => !string.IsNullOrEmpty(x.CompanyWebsite));

        RuleFor(x => x.TechFocus)
            .MaximumLength(500)
            .When(x => x.TechFocus != null);

        RuleFor(x => x.StatusId)
            .GreaterThan(0).WithMessage("'Status Id' must be a positive number.")
            .When(x => x.StatusId.HasValue);

        RuleFor(x => x.RecruiterId)
            .GreaterThan(0).WithMessage("'Recruiter Id' must be a positive number.")
            .When(x => x.RecruiterId.HasValue);
    }

    private static bool BeAValidUrl(string? url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
