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

public class StudentServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<StudentService>> _loggerMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly StudentService _sut; // System Under Test

    public StudentServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<StudentService>>();
        _notificationServiceMock = new Mock<INotificationService>();

        _sut = new StudentService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _loggerMock.Object,
            _notificationServiceMock.Object
        );
    }

    [Fact]
    public async Task GetStudentByIdAsync_WhenStudentExists_ReturnsSuccessResponse()
    {
        // Arrange
        var studentId = 1;
        var student = new Student
        {
            Id = studentId,
            StudentNumber = "STU202300001",
            GPA = 3.5m,
            User = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            }
        };

        var studentDto = new StudentDetailDto
        {
            Id = studentId,
            StudentNumber = "STU202300001",
            FullName = "John Doe",
            Email = "john.doe@example.com",
            GPA = 3.5m
        };

        _unitOfWorkMock.Setup(u => u.Students.GetByIdAsync(studentId, default))
            .ReturnsAsync(student);

        _mapperMock.Setup(m => m.Map<StudentDetailDto>(student))
            .Returns(studentDto);

        // Act
        var result = await _sut.GetStudentByIdAsync(studentId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(studentId);
        result.Data.StudentNumber.Should().Be("STU202300001");
        result.Data.GPA.Should().Be(3.5m);

        _unitOfWorkMock.Verify(u => u.Students.GetByIdAsync(studentId, default), Times.Once);
    }

    [Fact]
    public async Task GetStudentByIdAsync_WhenStudentDoesNotExist_ReturnsErrorResponse()
    {
        // Arrange
        var studentId = 999;
        _unitOfWorkMock.Setup(u => u.Students.GetByIdAsync(studentId, default))
            .ReturnsAsync((Student?)null);

        // Act
        var result = await _sut.GetStudentByIdAsync(studentId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Message.Should().Be("Student not found");
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task CreateStudentAsync_WithValidRequest_CreatesStudentAndReturnsSuccess()
    {
        // Arrange
        var request = new CreateStudentRequestDto
        {
            Email = "jane.doe@example.com",
            FirstName = "Jane",
            LastName = "Doe",
            DateOfBirth = new DateTime(2000, 1, 1),
            Major = "Computer Science"
        };

        var user = new User
        {
            Id = 2,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var student = new Student
        {
            Id = 1,
            UserId = 2,
            StudentNumber = "STU202300002",
            Major = "Computer Science",
            User = user
        };

        var studentDto = new StudentDetailDto
        {
            Id = 1,
            StudentNumber = "STU202300002",
            FullName = "Jane Doe",
            Email = request.Email
        };

        _unitOfWorkMock.Setup(u => u.Users.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), default))
            .ReturnsAsync(new List<User>());

        _unitOfWorkMock.Setup(u => u.Users.AddAsync(It.IsAny<User>(), default))
            .ReturnsAsync(user);

        _unitOfWorkMock.Setup(u => u.Students.AddAsync(It.IsAny<Student>(), default))
            .ReturnsAsync(student);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default))
            .ReturnsAsync(1);

        _mapperMock.Setup(m => m.Map<StudentDetailDto>(It.IsAny<Student>()))
            .Returns(studentDto);

        // Act
        var result = await _sut.CreateStudentAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be(request.Email);
        result.Message.Should().Be("Student created successfully");

        _unitOfWorkMock.Verify(u => u.Users.AddAsync(It.IsAny<User>(), default), Times.Once);
        _unitOfWorkMock.Verify(u => u.Students.AddAsync(It.IsAny<Student>(), default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Exactly(2));
    }

    [Fact]
    public async Task CreateStudentAsync_WithDuplicateEmail_ReturnsErrorResponse()
    {
        // Arrange
        var request = new CreateStudentRequestDto
        {
            Email = "existing@example.com",
            FirstName = "Test",
            LastName = "User"
        };

        var existingUser = new User { Id = 1, Email = request.Email };

        _unitOfWorkMock.Setup(u => u.Users.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), default))
            .ReturnsAsync(new List<User> { existingUser });

        // Act
        var result = await _sut.CreateStudentAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Message.Should().Be("Email already exists");

        _unitOfWorkMock.Verify(u => u.Users.AddAsync(It.IsAny<User>(), default), Times.Never);
        _unitOfWorkMock.Verify(u => u.Students.AddAsync(It.IsAny<Student>(), default), Times.Never);
    }

    [Fact]
    public async Task EnrollStudentAsync_WithValidRequest_EnrollsStudentAndSendsNotification()
    {
        // Arrange
        var request = new EnrollStudentRequestDto
        {
            StudentId = 1,
            CourseId = 1
        };

        var student = new Student { Id = 1, StudentNumber = "STU001" };
        var course = new Course
        {
            Id = 1,
            CourseName = "Mathematics",
            CourseCode = "MATH101",
            MaxStudents = 30
        };

        _unitOfWorkMock.Setup(u => u.Students.GetByIdAsync(request.StudentId, default))
            .ReturnsAsync(student);

        _unitOfWorkMock.Setup(u => u.Courses.GetByIdAsync(request.CourseId, default))
            .ReturnsAsync(course);

        _unitOfWorkMock.Setup(u => u.Enrollments.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Enrollment, bool>>>(), default))
            .ReturnsAsync(new List<Enrollment>());

        _unitOfWorkMock.Setup(u => u.Enrollments.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Enrollment, bool>>>(), default))
            .ReturnsAsync(15);

        _unitOfWorkMock.Setup(u => u.Enrollments.AddAsync(It.IsAny<Enrollment>(), default))
            .ReturnsAsync(new Enrollment());

        _notificationServiceMock.Setup(n => n.NotifyEnrollmentConfirmationAsync(request.StudentId, request.CourseId, default))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.EnrollStudentAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Student enrolled successfully");

        _unitOfWorkMock.Verify(u => u.Enrollments.AddAsync(It.IsAny<Enrollment>(), default), Times.Once);
        _notificationServiceMock.Verify(n => n.NotifyEnrollmentConfirmationAsync(request.StudentId, request.CourseId, default), Times.Once);
    }

    [Fact]
    public async Task EnrollStudentAsync_WhenCourseIsFull_ReturnsErrorResponse()
    {
        // Arrange
        var request = new EnrollStudentRequestDto
        {
            StudentId = 1,
            CourseId = 1
        };

        var student = new Student { Id = 1 };
        var course = new Course { Id = 1, MaxStudents = 30 };

        _unitOfWorkMock.Setup(u => u.Students.GetByIdAsync(request.StudentId, default))
            .ReturnsAsync(student);

        _unitOfWorkMock.Setup(u => u.Courses.GetByIdAsync(request.CourseId, default))
            .ReturnsAsync(course);

        _unitOfWorkMock.Setup(u => u.Enrollments.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Enrollment, bool>>>(), default))
            .ReturnsAsync(new List<Enrollment>());

        _unitOfWorkMock.Setup(u => u.Enrollments.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Enrollment, bool>>>(), default))
            .ReturnsAsync(30); // Course is full

        // Act
        var result = await _sut.EnrollStudentAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Message.Should().Be("Course is full");

        _unitOfWorkMock.Verify(u => u.Enrollments.AddAsync(It.IsAny<Enrollment>(), default), Times.Never);
    }

    [Fact]
    public async Task DeleteStudentAsync_WithValidId_DeletesStudentAndReturnsSuccess()
    {
        // Arrange
        var studentId = 1;
        var deletedBy = "admin@example.com";
        var student = new Student { Id = studentId };

        _unitOfWorkMock.Setup(u => u.Students.GetByIdAsync(studentId, default))
            .ReturnsAsync(student);

        _unitOfWorkMock.Setup(u => u.Students.SoftDeleteAsync(studentId, deletedBy, default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteStudentAsync(studentId, deletedBy);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Student deleted successfully");

        _unitOfWorkMock.Verify(u => u.Students.SoftDeleteAsync(studentId, deletedBy, default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }
}
