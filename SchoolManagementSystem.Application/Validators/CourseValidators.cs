using FluentValidation;
using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.Application.Validators;

public class CreateCourseRequestValidator : AbstractValidator<CreateCourseRequestDto>
{
    public CreateCourseRequestValidator()
    {
        RuleFor(x => x.CourseCode)
            .NotEmpty().WithMessage("Course code is required")
            .MaximumLength(20).WithMessage("Course code must not exceed 20 characters")
            .Matches(@"^[A-Z]{2,4}\d{3,4}$").WithMessage("Course code must be in format: CS101");

        RuleFor(x => x.CourseName)
            .NotEmpty().WithMessage("Course name is required")
            .MaximumLength(200).WithMessage("Course name must not exceed 200 characters");

        RuleFor(x => x.Credits)
            .InclusiveBetween(1, 6).WithMessage("Credits must be between 1 and 6");

        RuleFor(x => x.MaxStudents)
            .GreaterThan(0).WithMessage("Max students must be greater than 0")
            .LessThanOrEqualTo(500).WithMessage("Max students cannot exceed 500");

        RuleFor(x => x.Level)
            .Must(level => new[] { "Undergraduate", "Graduate", "Doctoral" }.Contains(level))
            .WithMessage("Invalid course level");
    }
}

public class UpdateCourseRequestValidator : AbstractValidator<UpdateCourseRequestDto>
{
    public UpdateCourseRequestValidator()
    {
        RuleFor(x => x.CourseName)
            .MaximumLength(200).WithMessage("Course name must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.CourseName));

        RuleFor(x => x.Credits)
            .InclusiveBetween(1, 6).WithMessage("Credits must be between 1 and 6")
            .When(x => x.Credits.HasValue);

        RuleFor(x => x.MaxStudents)
            .GreaterThan(0).WithMessage("Max students must be greater than 0")
            .LessThanOrEqualTo(500).WithMessage("Max students cannot exceed 500")
            .When(x => x.MaxStudents.HasValue);
    }
}
