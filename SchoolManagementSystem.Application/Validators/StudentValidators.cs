using FluentValidation;
using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.Application.Validators;

public class CreateStudentRequestValidator : AbstractValidator<CreateStudentRequestDto>
{
    public CreateStudentRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past")
            .When(x => x.DateOfBirth.HasValue);

        RuleFor(x => x.EmergencyContactPhone)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrEmpty(x.EmergencyContactPhone));
    }
}

public class UpdateStudentRequestValidator : AbstractValidator<UpdateStudentRequestDto>
{
    public UpdateStudentRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.FirstName));

        RuleFor(x => x.LastName)
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.LastName));

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past")
            .When(x => x.DateOfBirth.HasValue);

        RuleFor(x => x.Status)
            .Must(status => new[] { "Active", "Probation", "Suspended", "Graduated", "Withdrawn" }.Contains(status))
            .WithMessage("Invalid status value")
            .When(x => !string.IsNullOrEmpty(x.Status));
    }
}

public class EnrollStudentRequestValidator : AbstractValidator<EnrollStudentRequestDto>
{
    public EnrollStudentRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .GreaterThan(0).WithMessage("Valid student ID is required");

        RuleFor(x => x.CourseId)
            .GreaterThan(0).WithMessage("Valid course ID is required");
    }
}
