# School Management System - API Documentation

## Overview

Enterprise-grade RESTful API for comprehensive school management operations including student enrollment, course management, grading, attendance tracking, and more.

**Base URL (Development)**: `http://localhost:5000`
**Base URL (Production)**: `https://api.schoolmanagement.com`

**API Version**: v1
**Authentication**: JWT Bearer Token

---

## Table of Contents

1. [Authentication](#authentication)
2. [Students API](#students-api)
3. [Teachers API](#teachers-api)
4. [Courses API](#courses-api)
5. [Grades API](#grades-api)
6. [Assignments API](#assignments-api)
7. [Attendance API](#attendance-api)
8. [Schedules API](#schedules-api)
9. [Error Handling](#error-handling)
10. [Rate Limiting](#rate-limiting)
11. [Pagination](#pagination)

---

## Authentication

All API endpoints (except `/auth/login` and `/auth/register`) require authentication using JWT Bearer tokens.

### Get Access Token

**Endpoint**: `POST /api/v1/auth/login`

**Request Body**:
```json
{
  "emailOrUsername": "john.doe@example.com",
  "password": "SecurePassword123!",
  "rememberMe": false
}
```

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "success": true,
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "xYzAbC123...",
    "expiresAt": "2025-11-13T11:30:00Z",
    "user": {
      "id": 1,
      "email": "john.doe@example.com",
      "username": "johndoe",
      "firstName": "John",
      "lastName": "Doe",
      "fullName": "John Doe",
      "roles": ["Student"],
      "isActive": true
    },
    "requiresTwoFactor": false
  }
}
```

### Using the Token

Include the token in the `Authorization` header for all subsequent requests:

```
Authorization: Bearer <your-access-token>
```

### Refresh Token

**Endpoint**: `POST /api/v1/auth/refresh-token`

**Request Body**:
```json
"xYzAbC123refreshToken..."
```

**Response**: Same as login response

---

## Students API

### List All Students

**Endpoint**: `GET /api/v1/students`

**Query Parameters**:
- `pageNumber` (int, default: 1)
- `pageSize` (int, default: 10, max: 100)
- `searchTerm` (string, optional)
- `sortBy` (string, optional: "name", "gpa", "enrollmentDate")
- `sortOrder` (string, optional: "asc", "desc")
- `major` (string, optional)
- `status` (string, optional: "Active", "Probation", "Graduated")

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Students retrieved successfully",
  "data": {
    "items": [
      {
        "id": 1,
        "studentNumber": "STU20241234",
        "fullName": "John Doe",
        "email": "john.doe@example.com",
        "gpa": 3.75,
        "currentSemester": 3,
        "status": "Active",
        "major": "Computer Science"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 5,
    "totalCount": 45,
    "hasPrevious": false,
    "hasNext": true
  }
}
```

### Get Student by ID

**Endpoint**: `GET /api/v1/students/{id}`

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Student retrieved successfully",
  "data": {
    "id": 1,
    "userId": 10,
    "studentNumber": "STU20241234",
    "firstName": "John",
    "lastName": "Doe",
    "fullName": "John Doe",
    "email": "john.doe@example.com",
    "dateOfBirth": "2003-05-15T00:00:00Z",
    "age": 21,
    "enrollmentDate": "2022-09-01T00:00:00Z",
    "gpa": 3.75,
    "currentSemester": 3,
    "major": "Computer Science",
    "minor": "Mathematics",
    "status": "Active",
    "totalCreditsEarned": 60,
    "totalCreditsRequired": 120,
    "address": "123 Main St",
    "city": "San Francisco",
    "state": "CA",
    "country": "USA",
    "postalCode": "94102",
    "emergencyContactName": "Jane Doe",
    "emergencyContactPhone": "+1-555-0123",
    "advisor": {
      "id": 5,
      "fullName": "Dr. Smith",
      "email": "smith@example.com",
      "specialization": "Computer Science"
    },
    "enrolledCourses": [
      {
        "id": 101,
        "courseCode": "CS301",
        "courseName": "Data Structures",
        "credits": 3
      }
    ],
    "grades": [
      {
        "id": 1001,
        "courseName": "CS301",
        "value": 92,
        "letterGrade": "A",
        "gradeDate": "2025-10-15T00:00:00Z"
      }
    ]
  }
}
```

### Create Student

**Endpoint**: `POST /api/v1/students`

**Authorization**: Admin, SuperAdmin only

**Request Body**:
```json
{
  "email": "jane.smith@example.com",
  "firstName": "Jane",
  "lastName": "Smith",
  "dateOfBirth": "2004-08-22",
  "major": "Business Administration",
  "minor": "Economics",
  "advisorId": 5,
  "address": "456 Oak Ave",
  "city": "Los Angeles",
  "state": "CA",
  "country": "USA",
  "postalCode": "90001",
  "emergencyContactName": "Robert Smith",
  "emergencyContactPhone": "+1-555-0456"
}
```

**Response** (201 Created):
```json
{
  "success": true,
  "message": "Student created successfully",
  "data": {
    "id": 2,
    "studentNumber": "STU20245678",
    "fullName": "Jane Smith",
    "email": "jane.smith@example.com",
    "status": "Active"
  }
}
```

### Update Student

**Endpoint**: `PUT /api/v1/students/{id}`

**Authorization**: Admin, SuperAdmin, or the student themselves

**Request Body** (partial update supported):
```json
{
  "firstName": "Jane",
  "lastName": "Smith-Johnson",
  "major": "Business Administration",
  "address": "789 New Street"
}
```

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Student updated successfully",
  "data": {
    "id": 2,
    "fullName": "Jane Smith-Johnson",
    "major": "Business Administration"
  }
}
```

### Delete Student

**Endpoint**: `DELETE /api/v1/students/{id}`

**Authorization**: SuperAdmin only

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Student deleted successfully",
  "data": true
}
```

### Enroll Student in Course

**Endpoint**: `POST /api/v1/students/{id}/enroll`

**Request Body**:
```json
{
  "studentId": 1,
  "courseId": 101,
  "semesterId": 5
}
```

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Student enrolled successfully",
  "data": {
    "enrollmentId": 5001,
    "studentId": 1,
    "courseId": 101,
    "enrollmentDate": "2025-11-13T10:00:00Z",
    "status": "Active"
  }
}
```

### Get Student Transcript

**Endpoint**: `GET /api/v1/students/{id}/transcript`

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Transcript retrieved successfully",
  "data": {
    "student": {
      "id": 1,
      "studentNumber": "STU20241234",
      "fullName": "John Doe"
    },
    "semesterGrades": [
      {
        "semesterName": "Fall 2024",
        "grades": [
          {
            "courseName": "Data Structures",
            "courseCode": "CS301",
            "value": 92,
            "letterGrade": "A",
            "credits": 3
          }
        ],
        "semesterGPA": 3.85,
        "creditsEarned": 15
      }
    ],
    "overallGPA": 3.75,
    "totalCreditsEarned": 60,
    "generatedAt": "2025-11-13T10:30:00Z"
  }
}
```

---

## Courses API

### List All Courses

**Endpoint**: `GET /api/v1/courses`

**Query Parameters**:
- `pageNumber` (int)
- `pageSize` (int)
- `searchTerm` (string)
- `departmentId` (int)
- `semesterId` (int)
- `level` (string: "Undergraduate", "Graduate")
- `isActive` (bool)

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Courses retrieved successfully",
  "data": {
    "items": [
      {
        "id": 101,
        "courseCode": "CS301",
        "courseName": "Data Structures and Algorithms",
        "description": "Advanced study of data structures...",
        "credits": 3,
        "departmentName": "Computer Science",
        "teacherName": "Dr. Smith",
        "maxStudents": 30,
        "currentEnrollment": 25,
        "level": "Undergraduate",
        "isActive": true,
        "semesterName": "Fall 2024"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 8,
    "totalCount": 75
  }
}
```

### Get Course Details

**Endpoint**: `GET /api/v1/courses/{id}`

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Course retrieved successfully",
  "data": {
    "id": 101,
    "courseCode": "CS301",
    "courseName": "Data Structures and Algorithms",
    "description": "Comprehensive study of fundamental data structures...",
    "credits": 3,
    "departmentName": "Computer Science",
    "teacherName": "Dr. John Smith",
    "maxStudents": 30,
    "currentEnrollment": 25,
    "level": "Undergraduate",
    "isActive": true,
    "semesterName": "Fall 2024",
    "syllabus": "Week 1: Arrays and Linked Lists...",
    "learningOutcomes": "Students will be able to...",
    "prerequisites": ["CS201", "CS202"],
    "courseFee": 1500.00,
    "teacher": {
      "id": 5,
      "fullName": "Dr. John Smith",
      "email": "smith@example.com",
      "specialization": "Algorithms"
    },
    "enrolledStudents": [
      {
        "id": 1,
        "studentNumber": "STU20241234",
        "fullName": "John Doe"
      }
    ],
    "assignments": [
      {
        "id": 1001,
        "title": "Homework 1: Arrays",
        "dueDate": "2025-11-20T23:59:59Z",
        "maxScore": 100
      }
    ],
    "schedules": [
      {
        "id": 501,
        "dayOfWeek": "Monday",
        "startTime": "09:00:00",
        "endTime": "10:30:00",
        "room": "CS-101",
        "building": "Science Building"
      }
    ]
  }
}
```

### Create Course

**Endpoint**: `POST /api/v1/courses`

**Authorization**: Admin, SuperAdmin

**Request Body**:
```json
{
  "courseCode": "CS401",
  "courseName": "Machine Learning",
  "description": "Introduction to machine learning algorithms...",
  "credits": 3,
  "departmentId": 1,
  "teacherId": 5,
  "maxStudents": 25,
  "level": "Graduate",
  "prerequisiteCourseIds": [101, 102],
  "syllabus": "Week 1: Introduction to ML...",
  "learningOutcomes": "Students will master ML fundamentals...",
  "semesterId": 5,
  "courseFee": 2000.00
}
```

**Response** (201 Created):
```json
{
  "success": true,
  "message": "Course created successfully",
  "data": {
    "id": 150,
    "courseCode": "CS401",
    "courseName": "Machine Learning",
    "credits": 3
  }
}
```

---

## Grades API

### Create Grade

**Endpoint**: `POST /api/v1/grades`

**Authorization**: Teacher, Admin, SuperAdmin

**Request Body**:
```json
{
  "studentId": 1,
  "courseId": 101,
  "enrollmentId": 5001,
  "assignmentId": 1001,
  "gradeType": "Assignment",
  "value": 92,
  "maxValue": 100,
  "weight": 1.0,
  "comments": "Excellent work!",
  "isPublished": true
}
```

**Response** (201 Created):
```json
{
  "success": true,
  "message": "Grade created successfully",
  "data": {
    "id": 10001,
    "studentName": "John Doe",
    "courseName": "Data Structures",
    "value": 92,
    "letterGrade": "A",
    "isPublished": true
  }
}
```

### Bulk Grade Submission

**Endpoint**: `POST /api/v1/grades/bulk`

**Authorization**: Teacher, Admin, SuperAdmin

**Request Body**:
```json
{
  "courseId": 101,
  "assignmentId": 1001,
  "gradeType": "Assignment",
  "maxValue": 100,
  "weight": 1.0,
  "studentGrades": [
    {
      "studentId": 1,
      "value": 92,
      "comments": "Excellent"
    },
    {
      "studentId": 2,
      "value": 88,
      "comments": "Very good"
    }
  ]
}
```

**Response** (201 Created):
```json
{
  "success": true,
  "message": "Grades submitted successfully",
  "data": {
    "totalGrades": 2,
    "successfulSubmissions": 2,
    "failedSubmissions": 0
  }
}
```

---

## Assignments API

### Create Assignment

**Endpoint**: `POST /api/v1/assignments`

**Authorization**: Teacher, Admin, SuperAdmin

**Request Body**:
```json
{
  "courseId": 101,
  "title": "Homework 1: Binary Search Trees",
  "description": "Implement a balanced binary search tree...",
  "instructions": "Submit your code as a .zip file...",
  "dueDate": "2025-11-20T23:59:59Z",
  "maxScore": 100,
  "weight": 1.0,
  "type": "Assignment",
  "allowLateSubmission": true,
  "latePenaltyPercentage": 10,
  "isPublished": true
}
```

**Response** (201 Created):
```json
{
  "success": true,
  "message": "Assignment created successfully",
  "data": {
    "id": 1001,
    "title": "Homework 1: Binary Search Trees",
    "dueDate": "2025-11-20T23:59:59Z",
    "isPublished": true
  }
}
```

### Submit Assignment

**Endpoint**: `POST /api/v1/assignments/{id}/submit`

**Authorization**: Student

**Request Body**:
```json
{
  "assignmentId": 1001,
  "submissionText": "My solution approach...",
  "fileUrl": "https://storage.example.com/submissions/123.zip",
  "fileName": "homework1_johndoe.zip",
  "fileSize": 2048576
}
```

**Response** (201 Created):
```json
{
  "success": true,
  "message": "Assignment submitted successfully",
  "data": {
    "id": 50001,
    "assignmentId": 1001,
    "studentId": 1,
    "submittedAt": "2025-11-15T14:30:00Z",
    "status": "Submitted",
    "isLate": false
  }
}
```

---

## Attendance API

### Mark Attendance

**Endpoint**: `POST /api/v1/attendance`

**Authorization**: Teacher, Admin, SuperAdmin

**Request Body**:
```json
{
  "studentId": 1,
  "courseId": 101,
  "scheduleId": 501,
  "date": "2025-11-13",
  "status": "Present",
  "remarks": "",
  "isExcused": false
}
```

**Response** (201 Created):
```json
{
  "success": true,
  "message": "Attendance marked successfully",
  "data": {
    "id": 20001,
    "studentName": "John Doe",
    "courseName": "Data Structures",
    "date": "2025-11-13T00:00:00Z",
    "status": "Present"
  }
}
```

### Bulk Attendance

**Endpoint**: `POST /api/v1/attendance/bulk`

**Authorization**: Teacher, Admin, SuperAdmin

**Request Body**:
```json
{
  "courseId": 101,
  "scheduleId": 501,
  "date": "2025-11-13",
  "attendances": [
    {
      "studentId": 1,
      "status": "Present"
    },
    {
      "studentId": 2,
      "status": "Absent",
      "remarks": "Notified in advance"
    }
  ]
}
```

**Response** (201 Created):
```json
{
  "success": true,
  "message": "Attendance marked for 25 students",
  "data": {
    "totalRecords": 25,
    "present": 22,
    "absent": 2,
    "late": 1
  }
}
```

### Get Attendance Report

**Endpoint**: `GET /api/v1/attendance/report`

**Query Parameters**:
- `studentId` (int, required)
- `courseId` (int, required)
- `startDate` (date, optional)
- `endDate` (date, optional)

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Attendance report retrieved successfully",
  "data": {
    "student": {
      "id": 1,
      "fullName": "John Doe"
    },
    "course": {
      "id": 101,
      "courseName": "Data Structures"
    },
    "totalClasses": 40,
    "present": 36,
    "absent": 3,
    "late": 1,
    "excused": 0,
    "attendancePercentage": 90.0,
    "attendanceRecords": [
      {
        "id": 20001,
        "date": "2025-11-13T00:00:00Z",
        "status": "Present"
      }
    ]
  }
}
```

---

## Error Handling

All API errors follow a consistent format:

### Error Response Format

```json
{
  "success": false,
  "message": "Error description",
  "statusCode": 400,
  "errors": [
    "Detailed error message 1",
    "Detailed error message 2"
  ],
  "timestamp": "2025-11-13T10:30:00Z",
  "path": "/api/v1/students",
  "traceId": "0HN1234567890ABCDEF"
}
```

### Common HTTP Status Codes

- **200 OK**: Request successful
- **201 Created**: Resource created successfully
- **400 Bad Request**: Invalid request data
- **401 Unauthorized**: Authentication required
- **403 Forbidden**: Insufficient permissions
- **404 Not Found**: Resource not found
- **409 Conflict**: Resource conflict (e.g., duplicate entry)
- **422 Unprocessable Entity**: Validation failed
- **429 Too Many Requests**: Rate limit exceeded
- **500 Internal Server Error**: Server error

---

## Rate Limiting

API requests are rate-limited to prevent abuse:

- **Per IP**: 100 requests per minute, 1000 requests per hour
- **Authenticated Users**: 200 requests per minute, 5000 requests per hour

Rate limit headers are included in all responses:

```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 2025-11-13T10:35:00Z
```

---

## Pagination

All list endpoints support pagination:

### Query Parameters

- `pageNumber`: Page number (default: 1)
- `pageSize`: Items per page (default: 10, max: 100)

### Response Format

```json
{
  "items": [...],
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 5,
  "totalCount": 45,
  "hasPrevious": false,
  "hasNext": true
}
```

---

## Versioning

API uses URL versioning:

- **Current Version**: v1
- **Base Path**: `/api/v1/`

Future versions will be available at `/api/v2/`, etc.

---

## Support

For API support, contact:
- **Email**: api-support@schoolmanagement.com
- **Documentation**: https://docs.schoolmanagement.com
- **Status Page**: https://status.schoolmanagement.com

---

**Last Updated**: 2025-11-13
**Version**: 1.0.0
