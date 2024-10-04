using SchoolManagementSystem.Interfaces.Actions;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.BusinessLogicLayer.Exceptions;

public static class Exceptions
{
    public static void CheckPersonNotNull(ISchoolMemberActions? person)
    {
        if (person == null) throw new ArgumentNullException(nameof(person), "Person cannot be null.");
    }

    public static void CheckCourseNotNull(Course? course)
    {
        if (course == null) throw new ArgumentNullException(nameof(course), "Course cannot be null.");
    }

    public static void CheckCoursesNotNull(List<Course>? courses)
    {
        if (courses == null || courses.Count == 0)
            throw new ArgumentNullException(nameof(courses), "Courses list cannot be null or empty.");
    }

    public static void CheckStudentsNotNull(List<Student?>? students)
    {
        if (students == null || students.Count == 0)
            throw new ArgumentNullException(nameof(students), "Students list cannot be null or empty.");
    }

    public static void CheckTeachersNotNull(List<Teacher?>? teachers)
    {
        if (teachers == null) throw new ArgumentNullException(nameof(teachers), "Teachers list cannot be null.");
    }

    public static void CheckHasPermission(object? user, Student student)
    {
        if (!HasPermission(user, student))
            throw new PermissionDeniedException("You do not have permission to access this action.");
    }

    private static bool HasPermission(object? user, Student student)
    {
        return user switch
        {
            Admin => true,
            Teacher teacher => student.GetEnrolledCourses()
                .Any(course => Course.GetAssignedTeacher() == teacher.GetTeacherId()),
            Student studentUser => studentUser.GetStudentId() == student.GetStudentId(),
            _ => false
        };
    }

    public static void CheckStudentNotNull(Student? student)
    {
        if (student == null) throw new ArgumentNullException(nameof(student), "Student cannot be null.");
    }

    public static void CheckHasPermission(object? user, bool isAdmin = false, bool isTeacherOrAdmin = false)
    {
        if (!HasPermission(user, isAdmin, isTeacherOrAdmin))
            throw new PermissionDeniedException("You do not have the required permissions.");
    }

    private static bool HasPermission(object? user, bool isAdmin, bool isTeacherOrAdmin)
    {
        if (isAdmin && user is Admin) return true;
        return isTeacherOrAdmin && user is Admin or Teacher;
    }

    public static void CheckUserPermission(IUser? user)
    {
        if (user is not Admin && user is not Teacher)
            throw new PermissionDeniedException("You do not have permission to update the course name.");
    }

    public static void CheckPermission(object? user)
    {
        if (!HasPermission(user))
            throw new UnauthorizedAccessException("You do not have permission to demonstrate actions.");
    }

    private static bool HasPermission(object? user)
    {
        if (user is Admin) return true;
        return user is Teacher or Student;
    }

    private class PermissionDeniedException : Exception
    {
        public PermissionDeniedException(string message) : base(message)
        {
        }
    }

    public class InvalidNameException : Exception
    {
        public InvalidNameException(string message) : base(message) { }
    }

    public class StudentNotFoundException : Exception
    {
        public StudentNotFoundException(string studentId)
            : base($"Student with ID {studentId} not found.")
        {
        }
    }

    public class CourseNotFoundException : Exception
    {
        public CourseNotFoundException(string courseId)
            : base($"Course with ID {courseId} not found.")
        {
        }
    }

    public class TeacherNotFoundException : Exception
    {
        public TeacherNotFoundException(string teacherId)
            : base($"Teacher with ID {teacherId} not found.")
        {
        }
    }
}