using FluentValidation;
using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.Core.Validators;

/// <summary>
/// Validator for creating a new grade
/// </summary>
public class CreateGradeRequestValidator : AbstractValidator<CreateGradeRequestDto>
{
    public CreateGradeRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .GreaterThan(0)
            .WithMessage("Student ID must be greater than 0");

        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0");

        RuleFor(x => x.GradeType)
            .NotEmpty()
            .WithMessage("Grade type is required")
            .Must(type => new[] { "Assignment", "Exam", "Midterm", "Final", "Project", "Quiz", "Participation" }.Contains(type))
            .WithMessage("Grade type must be one of: Assignment, Exam, Midterm, Final, Project, Quiz, Participation");

        RuleFor(x => x.Value)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Grade value must be greater than or equal to 0")
            .LessThanOrEqualTo(x => x.MaxValue)
            .WithMessage("Grade value cannot exceed max value");

        RuleFor(x => x.MaxValue)
            .GreaterThan(0)
            .WithMessage("Max value must be greater than 0");

        RuleFor(x => x.Weight)
            .GreaterThan(0)
            .WithMessage("Weight must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Weight cannot exceed 100");

        RuleFor(x => x.Comments)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Comments))
            .WithMessage("Comments must not exceed 1000 characters");
    }
}

/// <summary>
/// Validator for updating an existing grade
/// </summary>
public class UpdateGradeRequestValidator : AbstractValidator<UpdateGradeRequestDto>
{
    public UpdateGradeRequestValidator()
    {
        RuleFor(x => x.Value)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Value.HasValue)
            .WithMessage("Grade value must be greater than or equal to 0");

        RuleFor(x => x.MaxValue)
            .GreaterThan(0)
            .When(x => x.MaxValue.HasValue)
            .WithMessage("Max value must be greater than 0");

        RuleFor(x => x.Weight)
            .GreaterThan(0)
            .When(x => x.Weight.HasValue)
            .WithMessage("Weight must be greater than 0")
            .LessThanOrEqualTo(100)
            .When(x => x.Weight.HasValue)
            .WithMessage("Weight cannot exceed 100");

        RuleFor(x => x.Comments)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Comments))
            .WithMessage("Comments must not exceed 1000 characters");
    }
}

/// <summary>
/// Validator for bulk grade creation
/// </summary>
public class BulkGradeRequestValidator : AbstractValidator<BulkGradeRequestDto>
{
    public BulkGradeRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0");

        RuleFor(x => x.GradeType)
            .NotEmpty()
            .WithMessage("Grade type is required")
            .Must(type => new[] { "Assignment", "Exam", "Midterm", "Final", "Project", "Quiz", "Participation" }.Contains(type))
            .WithMessage("Grade type must be one of: Assignment, Exam, Midterm, Final, Project, Quiz, Participation");

        RuleFor(x => x.MaxValue)
            .GreaterThan(0)
            .WithMessage("Max value must be greater than 0");

        RuleFor(x => x.Weight)
            .GreaterThan(0)
            .WithMessage("Weight must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Weight cannot exceed 100");

        RuleFor(x => x.StudentGrades)
            .NotEmpty()
            .WithMessage("Student grades list cannot be empty")
            .Must(list => list.Count <= 500)
            .WithMessage("Cannot create more than 500 grades at once");

        RuleForEach(x => x.StudentGrades).ChildRules(studentGrade =>
        {
            studentGrade.RuleFor(sg => sg.StudentId)
                .GreaterThan(0)
                .WithMessage("Student ID must be greater than 0");

            studentGrade.RuleFor(sg => sg.Value)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Grade value must be greater than or equal to 0");

            studentGrade.RuleFor(sg => sg.Comments)
                .MaximumLength(1000)
                .When(sg => !string.IsNullOrEmpty(sg.Comments))
                .WithMessage("Comments must not exceed 1000 characters");
        });
    }
}
