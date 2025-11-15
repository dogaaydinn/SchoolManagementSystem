using FluentValidation;
using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.Core.Validators;

/// <summary>
/// Validator for creating a new course
/// </summary>
public class CreateCourseRequestValidator : AbstractValidator<CreateCourseRequestDto>
{
    public CreateCourseRequestValidator()
    {
        RuleFor(x => x.CourseCode)
            .NotEmpty()
            .WithMessage("Course code is required")
            .MaximumLength(20)
            .WithMessage("Course code must not exceed 20 characters")
            .Matches(@"^[A-Z]{2,4}[0-9]{3,4}$")
            .WithMessage("Course code must follow format: 2-4 uppercase letters followed by 3-4 digits (e.g., CS101, MATH2301)");

        RuleFor(x => x.CourseName)
            .NotEmpty()
            .WithMessage("Course name is required")
            .MaximumLength(200)
            .WithMessage("Course name must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description must not exceed 2000 characters");

        RuleFor(x => x.Credits)
            .InclusiveBetween(1, 12)
            .WithMessage("Credits must be between 1 and 12");

        RuleFor(x => x.MaxStudents)
            .GreaterThan(0)
            .WithMessage("Max students must be greater than 0")
            .LessThanOrEqualTo(500)
            .WithMessage("Max students cannot exceed 500");

        RuleFor(x => x.Level)
            .Must(level => new[] { "Undergraduate", "Graduate", "Doctoral", "Certificate" }.Contains(level))
            .WithMessage("Level must be one of: Undergraduate, Graduate, Doctoral, Certificate");

        RuleFor(x => x.Syllabus)
            .MaximumLength(10000)
            .When(x => !string.IsNullOrEmpty(x.Syllabus))
            .WithMessage("Syllabus must not exceed 10000 characters");

        RuleFor(x => x.LearningOutcomes)
            .MaximumLength(5000)
            .When(x => !string.IsNullOrEmpty(x.LearningOutcomes))
            .WithMessage("Learning outcomes must not exceed 5000 characters");

        RuleFor(x => x.CourseFee)
            .GreaterThanOrEqualTo(0)
            .When(x => x.CourseFee.HasValue)
            .WithMessage("Course fee must be greater than or equal to 0");
    }
}

/// <summary>
/// Validator for updating an existing course
/// </summary>
public class UpdateCourseRequestValidator : AbstractValidator<UpdateCourseRequestDto>
{
    public UpdateCourseRequestValidator()
    {
        RuleFor(x => x.CourseName)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.CourseName))
            .WithMessage("Course name must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description must not exceed 2000 characters");

        RuleFor(x => x.Credits)
            .InclusiveBetween(1, 12)
            .When(x => x.Credits.HasValue)
            .WithMessage("Credits must be between 1 and 12");

        RuleFor(x => x.MaxStudents)
            .GreaterThan(0)
            .When(x => x.MaxStudents.HasValue)
            .WithMessage("Max students must be greater than 0")
            .LessThanOrEqualTo(500)
            .When(x => x.MaxStudents.HasValue)
            .WithMessage("Max students cannot exceed 500");

        RuleFor(x => x.Syllabus)
            .MaximumLength(10000)
            .When(x => !string.IsNullOrEmpty(x.Syllabus))
            .WithMessage("Syllabus must not exceed 10000 characters");

        RuleFor(x => x.LearningOutcomes)
            .MaximumLength(5000)
            .When(x => !string.IsNullOrEmpty(x.LearningOutcomes))
            .WithMessage("Learning outcomes must not exceed 5000 characters");
    }
}

/// <summary>
/// Validator for enrolling a student in a course
/// </summary>
public class EnrollStudentRequestValidator : AbstractValidator<EnrollStudentRequestDto>
{
    public EnrollStudentRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .GreaterThan(0)
            .WithMessage("Student ID must be greater than 0");

        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0");
    }
}
