using FluentValidation;
using JobTracker.Application.Dtos;

namespace JobTracker.Application.Validators;

public class CreateQuestionDtoValidator : AbstractValidator<CreateQuestionDto>
{
    public CreateQuestionDtoValidator()
    {
        RuleFor(x => x.QuestionText)
            .NotEmpty().WithMessage("Question text is required.");

        RuleFor(x => x.QuestionTypeId)
            .GreaterThan(0).WithMessage("A valid question type is required.");

        RuleForEach(x => x.TechTags)
            .MaximumLength(100).WithMessage("Each tech tag must not exceed 100 characters.")
            .When(x => x.TechTags != null);
    }
}

public class UpdateQuestionDtoValidator : AbstractValidator<UpdateQuestionDto>
{
    public UpdateQuestionDtoValidator()
    {
        RuleFor(x => x.QuestionText)
            .NotEmpty().WithMessage("Question text is required.");

        RuleFor(x => x.QuestionTypeId)
            .GreaterThan(0).WithMessage("A valid question type is required.");

        RuleForEach(x => x.TechTags)
            .MaximumLength(100).WithMessage("Each tech tag must not exceed 100 characters.")
            .When(x => x.TechTags != null);
    }
}
