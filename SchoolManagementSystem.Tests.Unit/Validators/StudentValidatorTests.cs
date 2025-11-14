using FluentAssertions;
using FluentValidation.TestHelper;
using SchoolManagementSystem.Application.Validators;
using SchoolManagementSystem.Core.DTOs;
using Xunit;

namespace SchoolManagementSystem.Tests.Unit.Validators;

public class StudentValidatorTests
{
    private readonly CreateStudentRequestValidator _validator;

    public StudentValidatorTests()
    {
        _validator = new CreateStudentRequestValidator();
    }

    [Fact]
    public void Validate_WithValidRequest_PassesValidation()
    {
        // Arrange
        var request = new CreateStudentRequestDto
        {
            Email = "student@example.com",
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(2000, 1, 1),
            PhoneNumber = "+12345678901",
            Major = "Computer Science"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Validate_WithInvalidEmail_FailsValidation(string email)
    {
        // Arrange
        var request = new CreateStudentRequestDto
        {
            Email = email,
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing@domain")]
    [InlineData("@example.com")]
    public void Validate_WithInvalidEmailFormat_FailsValidation(string email)
    {
        // Arrange
        var request = new CreateStudentRequestDto
        {
            Email = email,
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email format");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WithEmptyFirstName_FailsValidation(string firstName)
    {
        // Arrange
        var request = new CreateStudentRequestDto
        {
            Email = "test@example.com",
            FirstName = firstName,
            LastName = "Doe"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_WithTooLongFirstName_FailsValidation()
    {
        // Arrange
        var request = new CreateStudentRequestDto
        {
            Email = "test@example.com",
            FirstName = new string('A', 101), // 101 characters
            LastName = "Doe"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_WithFutureDateOfBirth_FailsValidation()
    {
        // Arrange
        var request = new CreateStudentRequestDto
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = DateTime.Today.AddDays(1)
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DateOfBirth)
            .WithErrorMessage("Date of birth must be in the past");
    }

    [Theory]
    [InlineData("123-456-7890")] // Invalid format
    [InlineData("abc")]           // Not a number
    [InlineData("+0000000000")]   // Invalid country code
    public void Validate_WithInvalidPhoneNumber_FailsValidation(string phoneNumber)
    {
        // Arrange
        var request = new CreateStudentRequestDto
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = phoneNumber
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData("+12345678901")]
    [InlineData("+447700900123")]
    [InlineData("+33123456789")]
    public void Validate_WithValidPhoneNumber_PassesValidation(string phoneNumber)
    {
        // Arrange
        var request = new CreateStudentRequestDto
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = phoneNumber
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void Validate_WithNullPhoneNumber_PassesValidation()
    {
        // Arrange - phone number is optional
        var request = new CreateStudentRequestDto
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = null
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }
}
