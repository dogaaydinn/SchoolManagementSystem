using FluentValidation;
using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.Core.Validators;

/// <summary>
/// Validator for creating a new student
/// </summary>
public class CreateStudentRequestValidator : AbstractValidator<CreateStudentRequestDto>
{
    public CreateStudentRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .MaximumLength(100)
            .WithMessage("Email must not exceed 100 characters");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(50)
            .WithMessage("First name must not exceed 50 characters")
            .Matches(@"^[a-zA-Z\s\-']+$")
            .WithMessage("First name contains invalid characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(50)
            .WithMessage("Last name must not exceed 50 characters")
            .Matches(@"^[a-zA-Z\s\-']+$")
            .WithMessage("Last name contains invalid characters");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow.AddYears(-16))
            .When(x => x.DateOfBirth.HasValue)
            .WithMessage("Student must be at least 16 years old");

        RuleFor(x => x.Major)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Major))
            .WithMessage("Major must not exceed 100 characters");

        RuleFor(x => x.Minor)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Minor))
            .WithMessage("Minor must not exceed 100 characters");

        RuleFor(x => x.PostalCode)
            .Matches(@"^\d{5}(-\d{4})?$")
            .When(x => !string.IsNullOrEmpty(x.PostalCode))
            .WithMessage("Invalid postal code format");

        RuleFor(x => x.EmergencyContactPhone)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .When(x => !string.IsNullOrEmpty(x.EmergencyContactPhone))
            .WithMessage("Invalid phone number format");
    }
}

/// <summary>
/// Validator for updating an existing student
/// </summary>
public class UpdateStudentRequestValidator : AbstractValidator<UpdateStudentRequestDto>
{
    public UpdateStudentRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.FirstName))
            .WithMessage("First name must not exceed 50 characters")
            .Matches(@"^[a-zA-Z\s\-']+$")
            .When(x => !string.IsNullOrEmpty(x.FirstName))
            .WithMessage("First name contains invalid characters");

        RuleFor(x => x.LastName)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.LastName))
            .WithMessage("Last name must not exceed 50 characters")
            .Matches(@"^[a-zA-Z\s\-']+$")
            .When(x => !string.IsNullOrEmpty(x.LastName))
            .WithMessage("Last name contains invalid characters");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow.AddYears(-16))
            .When(x => x.DateOfBirth.HasValue)
            .WithMessage("Student must be at least 16 years old");

        RuleFor(x => x.Status)
            .Must(status => new[] { "Active", "Inactive", "Graduated", "Suspended", "Withdrawn" }.Contains(status!))
            .When(x => !string.IsNullOrEmpty(x.Status))
            .WithMessage("Status must be one of: Active, Inactive, Graduated, Suspended, Withdrawn");

        RuleFor(x => x.Major)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Major))
            .WithMessage("Major must not exceed 100 characters");

        RuleFor(x => x.Minor)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Minor))
            .WithMessage("Minor must not exceed 100 characters");

        RuleFor(x => x.PostalCode)
            .Matches(@"^\d{5}(-\d{4})?$")
            .When(x => !string.IsNullOrEmpty(x.PostalCode))
            .WithMessage("Invalid postal code format");

        RuleFor(x => x.EmergencyContactPhone)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .When(x => !string.IsNullOrEmpty(x.EmergencyContactPhone))
            .WithMessage("Invalid phone number format");
    }
}
