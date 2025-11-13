# School Management System - Architecture Documentation

## System Overview

The School Management System is built using **Clean Architecture** principles with a focus on separation of concerns, testability, and maintainability. The system follows enterprise-level best practices suitable for production environments handling thousands of concurrent users.

---

## Table of Contents

1. [High-Level Architecture](#high-level-architecture)
2. [Layered Architecture](#layered-architecture)
3. [Technology Stack](#technology-stack)
4. [Design Patterns](#design-patterns)
5. [Database Design](#database-design)
6. [Security Architecture](#security-architecture)
7. [Scalability & Performance](#scalability--performance)
8. [Deployment Architecture](#deployment-architecture)

---

## High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                         Client Layer                            │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐         │
│  │  Admin Web   │  │  Teacher Web │  │  Student Web │         │
│  │  Dashboard   │  │    Portal    │  │    Portal    │         │
│  │   (React)    │  │   (React)    │  │   (React)    │         │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘         │
│         │                  │                  │                  │
│         └──────────────────┼──────────────────┘                 │
│                            │                                     │
└────────────────────────────┼─────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                      API Gateway / Load Balancer                │
│                   (Nginx / Azure App Gateway)                   │
└────────────────────────────┬─────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                     Application Layer                           │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │           ASP.NET Core Web API (REST)                    │  │
│  │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐       │  │
│  │  │ Controllers │ │ Middleware  │ │   Filters   │       │  │
│  │  └─────┬───────┘ └─────┬───────┘ └─────┬───────┘       │  │
│  │        │               │               │                 │  │
│  │        └───────────────┼───────────────┘                 │  │
│  │                        ▼                                  │  │
│  │            ┌───────────────────────┐                     │  │
│  │            │  Application Services │                     │  │
│  │            │   (Business Logic)    │                     │  │
│  │            └───────────┬───────────┘                     │  │
│  └────────────────────────┼─────────────────────────────────┘  │
└───────────────────────────┼─────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────────┐
│                    Data Access Layer                            │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐       │  │
│  │  │ Repositories│ │ Unit of Work│ │   EF Core   │       │  │
│  │  └─────┬───────┘ └─────┬───────┘ └─────┬───────┘       │  │
│  │        └───────────────┼───────────────┘                 │  │
│  └────────────────────────┼─────────────────────────────────┘  │
└───────────────────────────┼─────────────────────────────────────┘
                            │
        ┌───────────────────┼───────────────────┐
        │                   │                   │
        ▼                   ▼                   ▼
┌──────────────┐    ┌──────────────┐    ┌──────────────┐
│  SQL Server  │    │  Redis Cache │    │  Blob Storage│
│  (Primary)   │    │  (Caching)   │    │  (Documents) │
└──────────────┘    └──────────────┘    └──────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                    Supporting Services                          │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐            │
│  │  Seq/ELK    │  │  Prometheus │  │   Grafana   │            │
│  │  (Logging)  │  │  (Metrics)  │  │(Monitoring) │            │
│  └─────────────┘  └─────────────┘  └─────────────┘            │
└─────────────────────────────────────────────────────────────────┘
```

---

## Layered Architecture

### 1. Presentation Layer (API)
**Project**: `SchoolManagementSystem.API`

**Responsibilities**:
- HTTP request/response handling
- Input validation
- Authentication/Authorization
- API documentation (Swagger)
- Rate limiting
- CORS configuration

**Key Components**:
```
SchoolManagementSystem.API/
├── Controllers/
│   └── v1/
│       ├── AuthController.cs
│       ├── StudentsController.cs
│       ├── TeachersController.cs
│       ├── CoursesController.cs
│       ├── GradesController.cs
│       ├── AttendanceController.cs
│       └── AssignmentsController.cs
├── Middleware/
│   ├── ExceptionHandlingMiddleware.cs
│   ├── RequestLoggingMiddleware.cs
│   └── AuthenticationMiddleware.cs
├── Filters/
│   ├── ValidateModelStateFilter.cs
│   └── AuthorizeRoleFilter.cs
└── Program.cs
```

### 2. Application Layer
**Project**: `SchoolManagementSystem.Application`

**Responsibilities**:
- Business logic orchestration
- Data transformation (DTOs)
- Validation
- CQRS commands/queries (optional)

**Key Components**:
```
SchoolManagementSystem.Application/
├── Services/
│   ├── StudentService.cs
│   ├── CourseService.cs
│   ├── GradeService.cs
│   └── NotificationService.cs
├── Validators/
│   ├── StudentValidator.cs
│   └── CourseValidator.cs
└── Mapping/
    └── AutoMapperProfile.cs
```

### 3. Domain Layer (Core)
**Project**: `SchoolManagementSystem.Core`

**Responsibilities**:
- Domain entities
- Business rules
- Domain interfaces
- DTOs
- Enums

**Key Components**:
```
SchoolManagementSystem.Core/
├── Entities/
│   ├── User.cs
│   ├── Student.cs
│   ├── Teacher.cs
│   ├── Course.cs
│   ├── Grade.cs
│   ├── Assignment.cs
│   └── Attendance.cs
├── Interfaces/
│   ├── IRepository.cs
│   ├── IUnitOfWork.cs
│   ├── IAuthService.cs
│   └── ICacheService.cs
├── DTOs/
│   ├── AuthDtos.cs
│   ├── StudentDtos.cs
│   └── CourseDtos.cs
└── Enums/
    ├── UserRole.cs
    └── EnrollmentStatus.cs
```

### 4. Infrastructure Layer
**Project**: `SchoolManagementSystem.Infrastructure`

**Responsibilities**:
- Data access implementation
- External services integration
- Caching
- Logging
- Identity management

**Key Components**:
```
SchoolManagementSystem.Infrastructure/
├── Data/
│   ├── ApplicationDbContext.cs
│   └── Migrations/
├── Repositories/
│   ├── Repository.cs
│   └── UnitOfWork.cs
├── Identity/
│   ├── AuthService.cs
│   └── TokenService.cs
├── Caching/
│   └── RedisCacheService.cs
└── Services/
    ├── EmailService.cs
    └── StorageService.cs
```

---

## Technology Stack

### Backend
- **.NET 6.0**: Modern, cross-platform framework
- **ASP.NET Core**: Web API framework
- **Entity Framework Core 6.0**: ORM for database access
- **ASP.NET Core Identity**: Authentication/Authorization
- **JWT Bearer**: Token-based authentication

### Database
- **SQL Server 2019+**: Primary relational database
- **Redis**: Distributed caching
- **Azure Blob Storage / AWS S3**: Document storage

### Logging & Monitoring
- **Serilog**: Structured logging
- **Seq**: Log aggregation and analysis
- **Application Insights / New Relic**: APM
- **Prometheus**: Metrics collection
- **Grafana**: Metrics visualization

### Testing
- **xUnit**: Unit testing framework
- **Moq**: Mocking framework
- **FluentAssertions**: Assertion library
- **Testcontainers**: Integration testing

### DevOps
- **Docker**: Containerization
- **Kubernetes**: Container orchestration
- **GitHub Actions**: CI/CD pipeline
- **Terraform**: Infrastructure as code

### Security
- **BCrypt.Net**: Password hashing
- **HTTPS/TLS 1.3**: Encryption in transit
- **Rate Limiting**: DDoS protection
- **CORS**: Cross-origin resource sharing
- **Security Headers**: OWASP recommended headers

---

## Design Patterns

### 1. Repository Pattern
Abstracts data access logic, providing a collection-like interface for domain objects.

```csharp
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}
```

### 2. Unit of Work Pattern
Maintains a list of objects affected by a business transaction and coordinates writing changes.

```csharp
public interface IUnitOfWork : IDisposable
{
    IRepository<Student> Students { get; }
    IRepository<Course> Courses { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
}
```

### 3. Dependency Injection
All dependencies are injected through constructor injection, promoting loose coupling.

```csharp
public class StudentController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StudentController> _logger;

    public StudentController(IUnitOfWork unitOfWork, ILogger<StudentController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
}
```

### 4. Factory Pattern
Used for creating complex objects with multiple configuration options.

### 5. Strategy Pattern
Used for authentication strategies (JWT, OAuth, etc.).

### 6. Middleware Pattern
Request/response pipeline processing.

---

## Database Design

### Entity Relationship Diagram

```
┌─────────────┐
│    User     │
├─────────────┤
│ Id (PK)     │
│ Email       │
│ FirstName   │
│ LastName    │
└──────┬──────┘
       │ 1:1
       ├──────────────────┬──────────────────┐
       │                  │                  │
       ▼                  ▼                  ▼
┌────────────┐     ┌────────────┐    ┌────────────┐
│  Student   │     │  Teacher   │    │   Admin    │
├────────────┤     ├────────────┤    ├────────────┤
│ Id (PK)    │     │ Id (PK)    │    │ Id (PK)    │
│ UserId(FK) │     │ UserId(FK) │    │ UserId(FK) │
│ StudentNo  │     │ EmployeeNo │    │ Role       │
│ GPA        │     │ Dept       │    │ Permissions│
└─────┬──────┘     └─────┬──────┘    └────────────┘
      │ M:N              │ 1:M
      │                  │
      │           ┌──────▼─────┐
      │           │   Course   │
      │           ├────────────┤
      │           │ Id (PK)    │
      │           │ CourseCode │
      │           │ TeacherFK  │
      │           │ Credits    │
      │           └──────┬─────┘
      │                  │
      │           ┌──────▼─────┐
      └───────────►Enrollment  │
                  ├────────────┤
                  │ Id (PK)    │
                  │ StudentFK  │
                  │ CourseFK   │
                  │ Status     │
                  └──────┬─────┘
                         │ 1:M
                  ┌──────▼─────┐
                  │   Grade    │
                  ├────────────┤
                  │ Id (PK)    │
                  │ StudentFK  │
                  │ CourseFK   │
                  │ Value      │
                  └────────────┘
```

### Key Tables

1. **Users**: Authentication and base user information
2. **Students**: Student-specific data and academic records
3. **Teachers**: Teacher profiles and employment details
4. **Courses**: Course catalog and details
5. **Enrollments**: Student-course relationships
6. **Grades**: Academic performance records
7. **Assignments**: Course assignments
8. **Attendance**: Class attendance tracking
9. **Schedules**: Timetables and class schedules
10. **AuditLogs**: System activity tracking

### Indexing Strategy

**Primary Indexes**:
- All Primary Keys (clustered)
- Foreign Keys (non-clustered)

**Performance Indexes**:
```sql
CREATE INDEX IX_Students_StudentNumber ON Students(StudentNumber);
CREATE INDEX IX_Students_GPA ON Students(GPA);
CREATE INDEX IX_Enrollments_StudentCourse ON Enrollments(StudentId, CourseId);
CREATE INDEX IX_Grades_StudentCourse ON Grades(StudentId, CourseId);
CREATE INDEX IX_Attendance_StudentCourseDate ON Attendance(StudentId, CourseId, Date);
```

---

## Security Architecture

### 1. Authentication Flow

```
User Login Request
      │
      ▼
┌──────────────────┐
│ Validate Credentials │
└──────┬───────────┘
       │
       ▼ (Success)
┌──────────────────┐
│ Generate JWT Token │
│ + Refresh Token   │
└──────┬───────────┘
       │
       ▼
┌──────────────────┐
│ Return Tokens to │
│     Client       │
└──────────────────┘

Subsequent Requests
      │
      ▼
┌──────────────────┐
│ Extract JWT from │
│ Authorization    │
│     Header       │
└──────┬───────────┘
       │
       ▼
┌──────────────────┐
│ Validate Token   │
│ - Signature      │
│ - Expiration     │
│ - Claims         │
└──────┬───────────┘
       │
       ▼ (Valid)
┌──────────────────┐
│ Execute Request  │
└──────────────────┘
```

### 2. Authorization Levels

**Role Hierarchy**:
```
SuperAdmin (Level 5) - Full system access
    │
    ├─ Admin (Level 4) - Management functions
    │
    ├─ Teacher (Level 3) - Course management
    │
    └─ Student (Level 2) - View own data
```

### 3. Security Features

- **Password Hashing**: BCrypt with cost factor 12
- **JWT Tokens**: RS256 algorithm, 15-minute expiry
- **Refresh Tokens**: 7-day expiry with rotation
- **Account Lockout**: 5 failed attempts = 30-minute lockout
- **Rate Limiting**: 100 req/min per IP
- **SQL Injection Prevention**: Parameterized queries via EF Core
- **XSS Protection**: Input sanitization
- **CSRF Protection**: Anti-forgery tokens
- **HTTPS Only**: TLS 1.3 enforced
- **Security Headers**: HSTS, X-Frame-Options, CSP

---

## Scalability & Performance

### 1. Horizontal Scaling

```
             ┌─────────────────┐
             │  Load Balancer  │
             └────────┬────────┘
                      │
        ┌─────────────┼─────────────┐
        │             │             │
        ▼             ▼             ▼
   ┌────────┐    ┌────────┐    ┌────────┐
   │ API-1  │    │ API-2  │    │ API-3  │
   └────────┘    └────────┘    └────────┘
        │             │             │
        └─────────────┼─────────────┘
                      │
                      ▼
              ┌──────────────┐
              │   Database   │
              │  (Read/Write │
              │   Replicas)  │
              └──────────────┘
```

### 2. Caching Strategy

**Cache Layers**:
1. **In-Memory Cache**: Frequently accessed data (configs)
2. **Redis Distributed Cache**: Shared across instances
3. **Database Query Cache**: EF Core second-level cache

**Cache Keys**:
```
students:list:page:{pageNumber}:size:{pageSize}
student:{id}
course:{id}
courses:department:{deptId}
```

**TTL (Time To Live)**:
- Static content: 24 hours
- User sessions: 15 minutes
- Course catalog: 1 hour
- Student lists: 30 minutes

### 3. Database Optimization

- **Connection Pooling**: Max 100 connections
- **Read Replicas**: Separate read-only databases
- **Partitioning**: Large tables by date/year
- **Indexing**: Strategic indexes on frequent queries
- **Query Optimization**: LINQ to SQL analysis

### 4. Performance Targets

| Metric | Target |
|--------|--------|
| API Response Time (p95) | < 200ms |
| Database Query Time | < 50ms |
| Page Load Time | < 2s |
| Throughput | > 1000 req/sec |
| Concurrent Users | > 10,000 |
| Uptime | > 99.9% |

---

## Deployment Architecture

### Development Environment
```
Developer Machine
├── Docker Desktop
│   ├── SQL Server Container
│   ├── Redis Container
│   ├── API Container
│   └── Seq Container
└── Visual Studio / VS Code
```

### Staging Environment
```
Azure / AWS
├── App Service / ECS
│   └── API Instances (2)
├── Azure SQL / RDS
│   └── Database (Staging)
├── Redis Cache
└── Application Insights
```

### Production Environment
```
Kubernetes Cluster
├── Ingress Controller (Nginx)
├── API Pods (Auto-scaling: 3-10)
├── Background Job Pods (2)
├── Azure SQL / RDS (HA)
│   ├── Primary
│   └── Read Replicas (2)
├── Redis Cluster (3 nodes)
├── Blob Storage
└── Monitoring Stack
    ├── Prometheus
    ├── Grafana
    └── ELK Stack
```

### CI/CD Pipeline

```
GitHub Push
    │
    ▼
┌────────────────┐
│ Build & Test   │
│ - Compile      │
│ - Unit Tests   │
│ - Code Coverage│
└────┬───────────┘
     │
     ▼
┌────────────────┐
│ Security Scan  │
│ - SAST         │
│ - Dependency   │
│ - Container    │
└────┬───────────┘
     │
     ▼
┌────────────────┐
│ Docker Build   │
│ - Multi-stage  │
│ - Push to ECR  │
└────┬───────────┘
     │
     ▼
┌────────────────┐
│ Deploy Staging │
│ - Run E2E Tests│
└────┬───────────┘
     │
     ▼ (Manual Approval)
┌────────────────┐
│ Deploy Prod    │
│ - Blue/Green   │
│ - Health Check │
└────────────────┘
```

---

## Monitoring & Observability

### 1. Logging

**Log Levels**:
- **Trace**: Detailed diagnostic
- **Debug**: Development information
- **Information**: General flow
- **Warning**: Abnormal events
- **Error**: Error events
- **Critical**: Fatal errors

**Log Structure** (Structured Logging):
```json
{
  "timestamp": "2025-11-13T10:30:00Z",
  "level": "Information",
  "message": "Student enrolled in course",
  "properties": {
    "studentId": 1,
    "courseId": 101,
    "userId": "john.doe",
    "ipAddress": "192.168.1.100",
    "traceId": "0HN123..."
  }
}
```

### 2. Metrics

**Key Metrics**:
- Request rate (req/sec)
- Response time (p50, p95, p99)
- Error rate (%)
- Database query time
- Cache hit rate (%)
- Memory usage
- CPU usage

### 3. Alerts

**Alert Rules**:
- API error rate > 5%
- Response time p95 > 500ms
- Database connection errors
- High memory usage > 90%
- Failed login attempts > 100/min
- Disk space < 10%

---

## Disaster Recovery

### Backup Strategy

**Database Backups**:
- Full backup: Daily at 2 AM UTC
- Differential backup: Every 6 hours
- Transaction log backup: Every 15 minutes
- Retention: 30 days

**Restore Procedures**:
1. Identify backup point
2. Restore full backup
3. Apply differential backup
4. Apply transaction logs
5. Verify data integrity

### High Availability

- **Database**: Always On Availability Groups
- **API**: Multi-instance deployment
- **Cache**: Redis Cluster with replication
- **Storage**: Geo-redundant storage

**RTO (Recovery Time Objective)**: < 1 hour
**RPO (Recovery Point Objective)**: < 15 minutes

---

## Conclusion

This architecture provides a robust, scalable, and maintainable foundation for the School Management System. It follows industry best practices and can handle enterprise-level loads while maintaining security and performance standards.

---

**Document Version**: 1.0
**Last Updated**: 2025-11-13
**Maintained By**: Development Team
