using SchoolManagementSystem.Interfaces;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.BusinessLogicLayer.Exceptions
{
    public static class Exceptions
    {
        public static class Expectations
        {
            public static void CheckPersonNotNull(IPersonActions? person)
            {
                if (person == null)
                {
                    throw new ArgumentNullException(nameof(person), "Person cannot be null.");
                }
            }

            public static void CheckCourseNotNull(Course? course)
            {
                if (course == null)
                {
                    throw new ArgumentNullException(nameof(course), "Course cannot be null.");
                }
            }

            public static void CheckCoursesNotNull(List<Course>? courses)
            {
                if (courses == null || courses.Count == 0)
                {
                    throw new ArgumentNullException(nameof(courses), "Courses list cannot be null or empty.");
                }
            }

            public static void CheckStudentsNotNull(List<Student?> students)
            {
                if (students == null || students.Count == 0)
                {
                    throw new ArgumentNullException(nameof(students), "Students list cannot be null or empty.");
                }
            }

            public static void CheckTeachersNotNull(List<Teacher> teachers)
            {
                if (teachers == null)
                {
                    throw new ArgumentNullException(nameof(teachers), "Teachers list cannot be null.");
                }
            }

            public static void CheckStudentNotNull(Student? student)
            {
                if (student == null)
                {
                    throw new ArgumentNullException(nameof(student), "Student cannot be null.");
                }
            }

            public static void GradeNotFoundException(string studentId, string courseId)
            {
                throw new Exception($"Grade not found for student ID {studentId} in course ID {courseId}.");
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
    }
}