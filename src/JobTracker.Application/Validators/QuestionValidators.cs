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
    }
}
