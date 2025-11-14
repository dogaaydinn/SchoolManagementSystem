using FluentValidation;
using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.Application.Validators;

public class CreateAssignmentRequestValidator : AbstractValidator<CreateAssignmentRequestDto>
{
    public CreateAssignmentRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0).WithMessage("Valid course ID is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future");

        RuleFor(x => x.MaxScore)
            .GreaterThan(0).WithMessage("Max score must be greater than 0")
            .LessThanOrEqualTo(1000).WithMessage("Max score seems unreasonably high");

        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("Weight must be greater than 0")
            .LessThanOrEqualTo(10).WithMessage("Weight cannot exceed 10");

        RuleFor(x => x.LatePenaltyPercentage)
            .InclusiveBetween(0, 100).WithMessage("Late penalty must be between 0 and 100 percent")
            .When(x => x.LatePenaltyPercentage.HasValue);

        RuleFor(x => x.Type)
            .Must(type => new[] { "Assignment", "Quiz", "Exam", "Project", "Essay" }.Contains(type))
            .WithMessage("Invalid assignment type");
    }
}

public class SubmitAssignmentRequestValidator : AbstractValidator<SubmitAssignmentRequestDto>
{
    public SubmitAssignmentRequestValidator()
    {
        RuleFor(x => x.AssignmentId)
            .GreaterThan(0).WithMessage("Valid assignment ID is required");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.FileUrl) || !string.IsNullOrEmpty(x.SubmissionText))
            .WithMessage("Either file URL or submission text is required");

        RuleFor(x => x.FileSize)
            .LessThanOrEqualTo(10485760).WithMessage("File size cannot exceed 10 MB")
            .When(x => x.FileSize.HasValue);
    }
}
