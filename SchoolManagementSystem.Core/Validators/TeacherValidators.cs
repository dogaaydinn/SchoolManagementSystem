using FluentValidation;
using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.Core.Validators;

/// <summary>
/// Validator for creating a new teacher
/// </summary>
public class CreateTeacherRequestValidator : AbstractValidator<CreateTeacherRequestDto>
{
    public CreateTeacherRequestValidator()
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
            .LessThan(DateTime.UtcNow.AddYears(-21))
            .When(x => x.DateOfBirth.HasValue)
            .WithMessage("Teacher must be at least 21 years old");

        RuleFor(x => x.Specialization)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Specialization))
            .WithMessage("Specialization must not exceed 100 characters");

        RuleFor(x => x.Qualification)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.Qualification))
            .WithMessage("Qualification must not exceed 200 characters");

        RuleFor(x => x.Salary)
            .GreaterThan(0)
            .WithMessage("Salary must be greater than 0");

        RuleFor(x => x.EmploymentType)
            .Must(type => new[] { "Full-Time", "Part-Time", "Contract", "Adjunct" }.Contains(type))
            .WithMessage("Employment type must be one of: Full-Time, Part-Time, Contract, Adjunct");

        RuleFor(x => x.HireDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.HireDate != default)
            .WithMessage("Hire date cannot be in the future");

        RuleFor(x => x.OfficeLocation)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.OfficeLocation))
            .WithMessage("Office location must not exceed 100 characters");

        RuleFor(x => x.OfficeHours)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.OfficeHours))
            .WithMessage("Office hours must not exceed 200 characters");
    }
}

/// <summary>
/// Validator for updating an existing teacher
/// </summary>
public class UpdateTeacherRequestValidator : AbstractValidator<UpdateTeacherRequestDto>
{
    public UpdateTeacherRequestValidator()
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

        RuleFor(x => x.Specialization)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Specialization))
            .WithMessage("Specialization must not exceed 100 characters");

        RuleFor(x => x.Qualification)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.Qualification))
            .WithMessage("Qualification must not exceed 200 characters");

        RuleFor(x => x.Salary)
            .GreaterThan(0)
            .When(x => x.Salary.HasValue)
            .WithMessage("Salary must be greater than 0");

        RuleFor(x => x.EmploymentType)
            .Must(type => new[] { "Full-Time", "Part-Time", "Contract", "Adjunct" }.Contains(type!))
            .When(x => !string.IsNullOrEmpty(x.EmploymentType))
            .WithMessage("Employment type must be one of: Full-Time, Part-Time, Contract, Adjunct");

        RuleFor(x => x.OfficeLocation)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.OfficeLocation))
            .WithMessage("Office location must not exceed 100 characters");

        RuleFor(x => x.OfficeHours)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.OfficeHours))
            .WithMessage("Office hours must not exceed 200 characters");

        RuleFor(x => x.Biography)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrEmpty(x.Biography))
            .WithMessage("Biography must not exceed 2000 characters");

        RuleFor(x => x.ResearchInterests)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.ResearchInterests))
            .WithMessage("Research interests must not exceed 1000 characters");
    }
}
