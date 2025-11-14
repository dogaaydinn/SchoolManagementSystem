# Integration Tests

This project contains integration tests for the School Management System API.

## Test Coverage

### Authentication API
- **AuthController** (6 tests)
  - User registration (valid/invalid)
  - User login (valid credentials/invalid)
  - Get current user (with token/without token)
  - Password validation

## Running Tests

```bash
# Run all integration tests
dotnet test SchoolManagementSystem.Tests.Integration

# Run specific controller tests
dotnet test --filter "FullyQualifiedName~AuthControllerTests"
```

## Test Setup

### CustomWebApplicationFactory
Uses in-memory database for isolated testing:
```csharp
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
{
    // Configures test environment with in-memory database
    // Seeds test data
    // Sets up test services
}
```

### Test Client
```csharp
private readonly HttpClient _client;

public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
{
    _client = factory.CreateClient();
}
```

## Test Patterns

### HTTP Request/Response Testing
```csharp
var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);
response.StatusCode.Should().Be(HttpStatusCode.OK);

var result = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
result.Success.Should().BeTrue();
```

### Authentication Testing
```csharp
_client.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", token);
```

## Database Isolation

Each test run uses a fresh in-memory database to ensure test isolation.

## Future Enhancements

- Add tests for all remaining controllers:
  - StudentsController
  - CoursesController
  - GradesController
  - AssignmentsController
  - AttendanceController
  - SchedulesController
  - DocumentsController
  - ReportsController
  - MonitoringController

- Add performance tests
- Add load tests
- Add security tests
