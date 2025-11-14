using FluentAssertions;
using FluentValidation.TestHelper;
using SchoolManagementSystem.Application.Validators;
using SchoolManagementSystem.Core.DTOs;
using Xunit;

namespace SchoolManagementSystem.Tests.Unit.Validators;

public class CourseValidatorTests
{
    private readonly CreateCourseRequestValidator _validator;

    public CourseValidatorTests()
    {
        _validator = new CreateCourseRequestValidator();
    }

    [Fact]
    public void Validate_WithValidRequest_PassesValidation()
    {
        // Arrange
        var request = new CreateCourseRequestDto
        {
            CourseCode = "CS101",
            CourseName = "Introduction to Computer Science",
            Credits = 3,
            MaxStudents = 30,
            DepartmentId = 1
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("CS101")]      // Valid
    [InlineData("MATH2420")]   // Valid
    [InlineData("PHYS304")]    // Valid
    [InlineData("ENG1010")]    // Valid
    public void Validate_WithValidCourseCode_PassesValidation(string courseCode)
    {
        // Arrange
        var request = new CreateCourseRequestDto
        {
            CourseCode = courseCode,
            CourseName = "Test Course",
            Credits = 3,
            MaxStudents = 30
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CourseCode);
    }

    [Theory]
    [InlineData("CS")]         // Too short
    [InlineData("cs101")]      // Lowercase
    [InlineData("C101")]       // Only 1 letter
    [InlineData("CS1")]        // Too few digits
    [InlineData("COURSE101")]  // Too many letters
    [InlineData("CS12345")]    // Too many digits
    public void Validate_WithInvalidCourseCode_FailsValidation(string courseCode)
    {
        // Arrange
        var request = new CreateCourseRequestDto
        {
            CourseCode = courseCode,
            CourseName = "Test Course",
            Credits = 3
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CourseCode)
            .WithErrorMessage("Course code must be in format: CS101");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(7)]
    public void Validate_WithInvalidCredits_FailsValidation(int credits)
    {
        // Arrange
        var request = new CreateCourseRequestDto
        {
            CourseCode = "CS101",
            CourseName = "Test Course",
            Credits = credits,
            MaxStudents = 30
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Credits);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(6)]
    public void Validate_WithValidCredits_PassesValidation(int credits)
    {
        // Arrange
        var request = new CreateCourseRequestDto
        {
            CourseCode = "CS101",
            CourseName = "Test Course",
            Credits = credits,
            MaxStudents = 30
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Credits);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(501)]
    public void Validate_WithInvalidMaxStudents_FailsValidation(int maxStudents)
    {
        // Arrange
        var request = new CreateCourseRequestDto
        {
            CourseCode = "CS101",
            CourseName = "Test Course",
            Credits = 3,
            MaxStudents = maxStudents
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxStudents);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    [InlineData(500)]
    public void Validate_WithValidMaxStudents_PassesValidation(int maxStudents)
    {
        // Arrange
        var request = new CreateCourseRequestDto
        {
            CourseCode = "CS101",
            CourseName = "Test Course",
            Credits = 3,
            MaxStudents = maxStudents
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MaxStudents);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WithEmptyCourseName_FailsValidation(string courseName)
    {
        // Arrange
        var request = new CreateCourseRequestDto
        {
            CourseCode = "CS101",
            CourseName = courseName,
            Credits = 3
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CourseName);
    }
}
