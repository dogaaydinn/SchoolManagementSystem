using FluentValidation;
using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.Application.Validators;

public class CreateGradeRequestValidator : AbstractValidator<CreateGradeRequestDto>
{
    public CreateGradeRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .GreaterThan(0).WithMessage("Valid student ID is required");

        RuleFor(x => x.CourseId)
            .GreaterThan(0).WithMessage("Valid course ID is required");

        RuleFor(x => x.Value)
            .InclusiveBetween(0, x => x.MaxValue)
            .WithMessage("Grade value must be between 0 and max value");

        RuleFor(x => x.MaxValue)
            .GreaterThan(0).WithMessage("Max value must be greater than 0")
            .LessThanOrEqualTo(1000).WithMessage("Max value seems unreasonably high");

        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("Weight must be greater than 0")
            .LessThanOrEqualTo(10).WithMessage("Weight cannot exceed 10");

        RuleFor(x => x.GradeType)
            .Must(type => new[] { "Assignment", "Exam", "Midterm", "Final", "Project", "Quiz" }.Contains(type))
            .WithMessage("Invalid grade type");
    }
}

public class BulkGradeRequestValidator : AbstractValidator<BulkGradeRequestDto>
{
    public BulkGradeRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0).WithMessage("Valid course ID is required");

        RuleFor(x => x.StudentGrades)
            .NotEmpty().WithMessage("At least one student grade is required");

        RuleForEach(x => x.StudentGrades).ChildRules(grade =>
        {
            grade.RuleFor(x => x.StudentId)
                .GreaterThan(0).WithMessage("Valid student ID is required");

            grade.RuleFor(x => x.Value)
                .GreaterThanOrEqualTo(0).WithMessage("Grade value cannot be negative");
        });
    }
}
