# Unit Tests

This project contains comprehensive unit tests for the School Management System.

## Test Coverage

### Services
- **StudentService** (8 tests)
  - Get student by ID (exists/not exists)
  - Create student (valid/duplicate email)
  - Enroll student (valid/course full)
  - Delete student

- **GradeService** (10 tests)
  - Letter grade calculation (12-point scale)
  - Grade creation with notifications
  - Bulk grade creation with transactions
  - Enrollment validation
  - GPA calculation
  - Grade distribution analysis

### Validators
- **StudentValidators** (10 tests)
  - Email validation (format, required)
  - Name validation (length, required)
  - Date of birth validation (not in future)
  - Phone number validation (E.164 format)

- **CourseValidators** (8 tests)
  - Course code format (CS101, MATH2420)
  - Credits range (1-6)
  - Max students range (1-500)
  - Required fields

## Running Tests

```bash
# Run all unit tests
dotnet test SchoolManagementSystem.Tests.Unit

# Run with coverage
dotnet test SchoolManagementSystem.Tests.Unit /p:CollectCoverage=true

# Run specific test
dotnet test --filter "FullyQualifiedName~StudentServiceTests"
```

## Test Patterns

### AAA Pattern
All tests follow the Arrange-Act-Assert pattern:
```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedResult()
{
    // Arrange
    var request = new SomeRequest { ... };

    // Act
    var result = await _sut.SomeMethod(request);

    // Assert
    result.Success.Should().BeTrue();
}
```

### Mocking with Moq
```csharp
_unitOfWorkMock.Setup(u => u.Students.GetByIdAsync(id, default))
    .ReturnsAsync(student);
```

### FluentAssertions
```csharp
result.Should().NotBeNull();
result.Success.Should().BeTrue();
result.Data.Should().NotBeNull();
result.Data!.GPA.Should().Be(3.5m);
```

## Test Data Builders

Consider using AutoFixture for complex test data:
```csharp
var fixture = new Fixture();
var student = fixture.Create<Student>();
```

## Coverage Goals

- **Target**: 80% overall code coverage
- **Critical paths**: 100% coverage
- **Services**: 90%+ coverage
- **Validators**: 100% coverage
