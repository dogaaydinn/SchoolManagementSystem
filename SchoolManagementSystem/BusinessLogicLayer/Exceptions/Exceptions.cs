using System;

namespace SchoolManagementSystem.BusinessLogicLayer.Exceptions
{
    public static class Exceptions
    {
        public class StudentNotFoundException : Exception
        {
            public StudentNotFoundException() 
                : base("Student not found.")
            {
            }

            public StudentNotFoundException(string studentId) 
                : base($"Student with ID {studentId} not found.")
            {
            }
        }

        public class CourseNotFoundException : Exception
        {
            public CourseNotFoundException() 
                : base("Course not found.")
            {
            }

            public CourseNotFoundException(string courseId) 
                : base($"Course with ID {courseId} not found.")
            {
            }
        }

        public class TeacherNotFoundException : Exception
        {
            public TeacherNotFoundException() 
                : base("Teacher not found.")
            {
            }

            public TeacherNotFoundException(string teacherId) 
                : base($"Teacher with ID {teacherId} not found.")
            {
            }
        }

        public class GradeNotFoundException : Exception
        {
            public GradeNotFoundException() 
                : base("Grade not found.")
            {
            }

            public GradeNotFoundException(string studentId, string courseId) 
                : base($"Grade not found for student ID {studentId} in course ID {courseId}.")
            {
            }
        }
    }
}