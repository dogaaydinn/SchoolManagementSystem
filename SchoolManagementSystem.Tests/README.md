# SchoolManagementSystem.Tests

Comprehensive test suite for the School Management System.

## Test Structure

```
SchoolManagementSystem.Tests/
├── Unit/                           # Unit tests
│   ├── Services/
│   │   ├── AuthServiceTests.cs
│   │   ├── TokenServiceTests.cs
│   │   └── ...
│   ├── Repositories/
│   │   ├── RepositoryTests.cs
│   │   └── UnitOfWorkTests.cs
│   ├── Controllers/
│   │   ├── AuthControllerTests.cs
│   │   └── ...
│   └── Validators/
│       ├── AuthValidatorsTests.cs
│       └── ...
├── Integration/                    # Integration tests
│   ├── AuthenticationFlowTests.cs
│   ├── DatabaseTests.cs
│   ├── CacheTests.cs
│   └── ...
├── E2E/                           # End-to-end tests
│   ├── StudentEnrollmentE2ETests.cs
│   └── ...
└── Fixtures/                      # Test fixtures and helpers
    ├── TestDataFixture.cs
    ├── DatabaseFixture.cs
    └── ...
```

## Running Tests

### Run all tests
```bash
dotnet test
```

### Run unit tests only
```bash
dotnet test --filter Category=Unit
```

### Run integration tests only
```bash
dotnet test --filter Category=Integration
```

### Run with code coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Run specific test class
```bash
dotnet test --filter FullyQualifiedName~AuthServiceTests
```

## Test Categories

Tests are categorized using the `[Trait]` attribute:

- `Category=Unit` - Fast, isolated unit tests
- `Category=Integration` - Tests that interact with external systems (DB, Redis)
- `Category=E2E` - Full end-to-end workflow tests

## Code Coverage Targets

- **Unit Tests**: >80%
- **Integration Tests**: >70%
- **Overall Coverage**: >75%

## Writing Tests

### Naming Convention

```
MethodName_StateUnderTest_ExpectedBehavior
```

Example:
```csharp
LoginAsync_WithValidCredentials_ShouldReturnAuthResponse()
```

### AAA Pattern

All tests follow the Arrange-Act-Assert pattern:

```csharp
[Fact]
public async Task ExampleTest()
{
    // Arrange
    var service = new MyService();
    var input = "test";

    // Act
    var result = await service.DoSomething(input);

    // Assert
    result.Should().NotBeNull();
    result.Value.Should().Be("expected");
}
```

### Using FluentAssertions

```csharp
result.Should().NotBeNull();
result.Should().BeOfType<AuthResponseDto>();
result.Success.Should().BeTrue();
result.Data!.AccessToken.Should().NotBeNullOrEmpty();
```

### Using Moq for Mocking

```csharp
var mockService = new Mock<IMyService>();
mockService.Setup(x => x.GetData(It.IsAny<int>()))
           .ReturnsAsync(new MyData());
```

## Test Data

Use the `TestDataFixture` class for consistent test data:

```csharp
public class MyTests : IClassFixture<TestDataFixture>
{
    private readonly TestDataFixture _fixture;

    public MyTests(TestDataFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void MyTest()
    {
        var testUser = _fixture.CreateTestStudent();
        // ...
    }
}
```

## Integration Tests

Integration tests use:
- **In-Memory Database** for fast database tests
- **Testcontainers** for testing with real database
- **WebApplicationFactory** for API integration tests

Example:
```csharp
public class DatabaseTests : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task CanSaveAndRetrieveUser()
    {
        // Uses real database via Testcontainers
    }
}
```

## CI/CD Integration

Tests are automatically run in the CI/CD pipeline:
- On every pull request
- On pushes to `main` and `develop` branches
- Code coverage reports are uploaded to Codecov

## Troubleshooting

### Tests timing out
- Increase timeout: `[Fact(Timeout = 5000)]`
- Check for deadlocks in async code

### Database tests failing
- Ensure database is properly cleaned between tests
- Use transactions for test isolation

### Flaky tests
- Avoid hard-coded delays (`Thread.Sleep`)
- Use proper async/await patterns
- Mock time-dependent operations

## Resources

- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Testcontainers Documentation](https://dotnet.testcontainers.org/)
