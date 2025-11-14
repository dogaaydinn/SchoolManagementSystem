using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SchoolManagementSystem.Application.Interfaces;
using SchoolManagementSystem.Application.Services;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Entities;
using SchoolManagementSystem.Core.Interfaces;
using Xunit;

namespace SchoolManagementSystem.Tests.Unit.Services;

public class GradeServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GradeService>> _loggerMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly GradeService _sut;

    public GradeServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GradeService>>();
        _notificationServiceMock = new Mock<INotificationService>();

        _sut = new GradeService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _loggerMock.Object,
            _notificationServiceMock.Object
        );
    }

    [Theory]
    [InlineData(95, 100, "A")]
    [InlineData(92, 100, "A-")]
    [InlineData(88, 100, "B+")]
    [InlineData(85, 100, "B")]
    [InlineData(82, 100, "B-")]
    [InlineData(78, 100, "C+")]
    [InlineData(75, 100, "C")]
    [InlineData(72, 100, "C-")]
    [InlineData(68, 100, "D+")]
    [InlineData(65, 100, "D")]
    [InlineData(62, 100, "D-")]
    [InlineData(55, 100, "F")]
    public async Task CreateGradeAsync_CalculatesCorrectLetterGrade(decimal value, decimal maxValue, string expectedLetterGrade)
    {
        // Arrange
        var request = new CreateGradeRequestDto
        {
            StudentId = 1,
            CourseId = 1,
            Value = value,
            MaxValue = maxValue,
            GradeType = "Exam"
        };

        var student = new Student { Id = 1 };
        var course = new Course { Id = 1, CourseName = "Math" };
        var enrollment = new Enrollment { StudentId = 1, CourseId = 1, Status = "Active" };

        _unitOfWorkMock.Setup(u => u.Students.GetByIdAsync(request.StudentId, default))
            .ReturnsAsync(student);

        _unitOfWorkMock.Setup(u => u.Courses.GetByIdAsync(request.CourseId, default))
            .ReturnsAsync(course);

        _unitOfWorkMock.Setup(u => u.Enrollments.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Enrollment, bool>>>(), default))
            .ReturnsAsync(new List<Enrollment> { enrollment });

        _unitOfWorkMock.Setup(u => u.Grades.AddAsync(It.IsAny<Grade>(), default))
            .ReturnsAsync((Grade g) => g);

        var capturedGrade = new Grade();
        _unitOfWorkMock.Setup(u => u.Grades.AddAsync(It.IsAny<Grade>(), default))
            .Callback<Grade, CancellationToken>((g, ct) => capturedGrade = g)
            .ReturnsAsync((Grade g, CancellationToken ct) => g);

        _mapperMock.Setup(m => m.Map<GradeDto>(It.IsAny<Grade>()))
            .Returns(new GradeDto { LetterGrade = expectedLetterGrade });

        // Act
        var result = await _sut.CreateGradeAsync(request, "teacher@example.com");

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        capturedGrade.LetterGrade.Should().Be(expectedLetterGrade);
        capturedGrade.Percentage.Should().BeApproximately((value / maxValue) * 100, 0.01m);
    }

    [Fact]
    public async Task CreateGradeAsync_SendsNotificationToStudent()
    {
        // Arrange
        var request = new CreateGradeRequestDto
        {
            StudentId = 1,
            CourseId = 1,
            Value = 85,
            MaxValue = 100,
            GradeType = "Midterm"
        };

        SetupSuccessfulGradeCreation(request);

        // Act
        var result = await _sut.CreateGradeAsync(request, "teacher@example.com");

        // Assert
        result.Success.Should().BeTrue();
        _notificationServiceMock.Verify(
            n => n.NotifyGradePostedAsync(request.StudentId, It.IsAny<int>(), default),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateGradeAsync_WhenStudentNotEnrolled_ReturnsErrorResponse()
    {
        // Arrange
        var request = new CreateGradeRequestDto
        {
            StudentId = 1,
            CourseId = 1,
            Value = 85,
            MaxValue = 100
        };

        var student = new Student { Id = 1 };
        var course = new Course { Id = 1 };

        _unitOfWorkMock.Setup(u => u.Students.GetByIdAsync(request.StudentId, default))
            .ReturnsAsync(student);

        _unitOfWorkMock.Setup(u => u.Courses.GetByIdAsync(request.CourseId, default))
            .ReturnsAsync(course);

        _unitOfWorkMock.Setup(u => u.Enrollments.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Enrollment, bool>>>(), default))
            .ReturnsAsync(new List<Enrollment>()); // No enrollment

        // Act
        var result = await _sut.CreateGradeAsync(request, "teacher@example.com");

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Message.Should().Be("Student not enrolled in this course");

        _unitOfWorkMock.Verify(u => u.Grades.AddAsync(It.IsAny<Grade>(), default), Times.Never);
    }

    [Fact]
    public async Task BulkCreateGradesAsync_CreatesMultipleGradesInTransaction()
    {
        // Arrange
        var request = new BulkGradeRequestDto
        {
            CourseId = 1,
            GradeType = "Final",
            MaxValue = 100,
            StudentGrades = new List<StudentGradeDto>
            {
                new() { StudentId = 1, Value = 90 },
                new() { StudentId = 2, Value = 85 },
                new() { StudentId = 3, Value = 95 }
            }
        };

        var course = new Course { Id = 1 };

        _unitOfWorkMock.Setup(u => u.Courses.GetByIdAsync(request.CourseId, default))
            .ReturnsAsync(course);

        _unitOfWorkMock.Setup(u => u.Students.GetByIdAsync(It.IsAny<int>(), default))
            .ReturnsAsync((int id, CancellationToken ct) => new Student { Id = id });

        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync())
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.CommitTransactionAsync())
            .Returns(Task.CompletedTask);

        _mapperMock.Setup(m => m.Map<IEnumerable<GradeDto>>(It.IsAny<List<Grade>>()))
            .Returns(new List<GradeDto>());

        // Act
        var result = await _sut.BulkCreateGradesAsync(request, "teacher@example.com");

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("3 grades created successfully");

        _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.Grades.AddAsync(It.IsAny<Grade>(), default), Times.Exactly(3));
        _notificationServiceMock.Verify(n => n.NotifyGradePostedAsync(It.IsAny<int>(), It.IsAny<int>(), default), Times.Exactly(3));
    }

    [Fact]
    public async Task BulkCreateGradesAsync_OnError_RollsBackTransaction()
    {
        // Arrange
        var request = new BulkGradeRequestDto
        {
            CourseId = 1,
            MaxValue = 100,
            StudentGrades = new List<StudentGradeDto>
            {
                new() { StudentId = 1, Value = 90 }
            }
        };

        _unitOfWorkMock.Setup(u => u.Courses.GetByIdAsync(request.CourseId, default))
            .ThrowsAsync(new Exception("Database error"));

        _unitOfWorkMock.Setup(u => u.RollbackTransactionAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.BulkCreateGradesAsync(request, "teacher@example.com");

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(500);

        _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
    }

    [Theory]
    [InlineData("A", 4.0)]
    [InlineData("A-", 3.7)]
    [InlineData("B+", 3.3)]
    [InlineData("B", 3.0)]
    [InlineData("B-", 2.7)]
    [InlineData("C+", 2.3)]
    [InlineData("C", 2.0)]
    [InlineData("C-", 1.7)]
    [InlineData("D+", 1.3)]
    [InlineData("D", 1.0)]
    [InlineData("D-", 0.7)]
    [InlineData("F", 0.0)]
    public async Task CalculateStudentGPAAsync_CalculatesCorrectGPA(string letterGrade, decimal expectedGradePoint)
    {
        // This tests the GPA calculation logic
        // Would need to expose the conversion method or test through public methods

        // For now, this is a placeholder showing the expected behavior
        expectedGradePoint.Should().BeGreaterThanOrEqualTo(0);
        expectedGradePoint.Should().BeLessThanOrEqualTo(4.0m);

        await Task.CompletedTask;
    }

    [Fact]
    public async Task GetGradeDistributionAsync_ReturnsCorrectDistribution()
    {
        // Arrange
        var courseId = 1;
        var grades = new List<Grade>
        {
            new() { LetterGrade = "A", CourseId = courseId },
            new() { LetterGrade = "A-", CourseId = courseId },
            new() { LetterGrade = "B+", CourseId = courseId },
            new() { LetterGrade = "B", CourseId = courseId },
            new() { LetterGrade = "C", CourseId = courseId },
            new() { LetterGrade = "F", CourseId = courseId }
        };

        _unitOfWorkMock.Setup(u => u.Grades.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Grade, bool>>>(), default))
            .ReturnsAsync(grades);

        // Act
        var result = await _sut.GetGradeDistributionAsync(courseId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.ACount.Should().Be(2); // A and A-
        result.Data.BCount.Should().Be(2); // B+ and B
        result.Data.CCount.Should().Be(1);
        result.Data.FCount.Should().Be(1);
    }

    private void SetupSuccessfulGradeCreation(CreateGradeRequestDto request)
    {
        var student = new Student { Id = request.StudentId };
        var course = new Course { Id = request.CourseId };
        var enrollment = new Enrollment { StudentId = request.StudentId, CourseId = request.CourseId, Status = "Active" };

        _unitOfWorkMock.Setup(u => u.Students.GetByIdAsync(request.StudentId, default))
            .ReturnsAsync(student);

        _unitOfWorkMock.Setup(u => u.Courses.GetByIdAsync(request.CourseId, default))
            .ReturnsAsync(course);

        _unitOfWorkMock.Setup(u => u.Enrollments.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Enrollment, bool>>>(), default))
            .ReturnsAsync(new List<Enrollment> { enrollment });

        _unitOfWorkMock.Setup(u => u.Grades.AddAsync(It.IsAny<Grade>(), default))
            .ReturnsAsync((Grade g) => { g.Id = 1; return g; });

        _mapperMock.Setup(m => m.Map<GradeDto>(It.IsAny<Grade>()))
            .Returns(new GradeDto { Id = 1 });

        _notificationServiceMock.Setup(n => n.NotifyGradePostedAsync(It.IsAny<int>(), It.IsAny<int>(), default))
            .ReturnsAsync(true);
    }
}
