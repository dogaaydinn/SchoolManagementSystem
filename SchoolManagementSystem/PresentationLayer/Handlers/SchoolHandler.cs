using SchoolManagementSystem.BusinessLogicLayer.Exceptions;
using SchoolManagementSystem.Interfaces;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.PresentationLayer.Handlers
{
    public static class SchoolHandler
    {
        public static void DisplayAllDetails(List<Course>? courses, List<Student?> students, List<Teacher> teachers)
        {
            Exceptions.Expectations.CheckCoursesNotNull(courses);
            Exceptions.Expectations.CheckStudentsNotNull(students);
            Exceptions.Expectations.CheckTeachersNotNull(teachers);

            Console.WriteLine("Courses:");
            CourseHandler.DisplayCourseDetails(courses);

            Console.WriteLine("\nStudents:");
            StudentHandler.DisplayStudentDetails(students, courses);

            Console.WriteLine("\nTeachers:");
            foreach (var teacher in teachers)
            {
                Console.WriteLine($"Teacher ID: {teacher.GetTeacherId()}, Name: {teacher.GetTeacherFullName()}, Subject: {teacher.GetSubject()}");
            }
        }

        public static void AssignCoursesToStudents(List<Course>? courses, List<Student?> students, IUser user)
        {
            Exceptions.Expectations.CheckCoursesNotNull(courses);
            Exceptions.Expectations.CheckStudentsNotNull(students);
            
            if (user is not Teacher && user is not Admin)
            {
                Console.WriteLine("You do not have permission to assign courses.");
                return;
            }

            foreach (var student in students.OfType<Student>())
            {
                Console.WriteLine($"Assigning courses to {student.GetStudentFullName()} (ID: {student.GetStudentId()})");

                while (true)
                {
                    Console.WriteLine("Enter the course ID to assign (or type 'done' to finish):");
                    var input = Console.ReadLine()?.Trim();

                    if (string.IsNullOrEmpty(input)) continue;

                    if (input.ToLower() == "done") break;

                    if (!int.TryParse(input, out var courseId))
                    {
                        Console.WriteLine("Invalid course ID. Please try again.");
                        continue;
                    }

                    var course = courses.Find(c => c.GetCourseId() == courseId);
                    if (course == null)
                    {
                        Console.WriteLine("Course not found. Please try again.");
                        continue;
                    }

                    course.EnrollStudent(student);
                    Console.WriteLine($"Assigned {course.GetCourseName()} to {student.GetStudentFullName()}.");
                }
            }
        }

        public static void RecordGradesForStudents(List<Course>? courses, IUser user)
        {
            Exceptions.Expectations.CheckCoursesNotNull(courses);
            
            if (user is not Teacher && user is not Admin)
            {
                Console.WriteLine("You do not have permission to record grades.");
                return;
            }

            foreach (var course in courses.OfType<Course>())
            {
                Console.WriteLine($"Recording grades for course: {course.GetCourseName()} (ID: {course.GetCourseId()})");

                foreach (var student in course.GetEnrolledStudents().OfType<Student>())
                {
                    if (student == null) continue;

                    Console.WriteLine($"Enter the grade for {student.GetStudentFullName()} (ID: {student.GetStudentId()}):");

                    var input = Console.ReadLine();

                    if (!double.TryParse(input, out var grade) || grade < 0 || grade > 100)
                    {
                        Console.WriteLine("Invalid grade. Please enter a value between 0 and 100.");
                        continue;
                    }

                    course.AssignGrade(student, grade);
                    Console.WriteLine($"Recorded grade {grade} for {student.GetStudentFullName()} in {course.GetCourseName()}.");
                }
            }
        }

        public static void DemonstrateActions(IPersonActions person)
        {
            switch (person)
            {
                case ITeacherActions teacher:
                    DemonstrateTeacherActions(teacher);
                    break;
                case IStudentActions student:
                    DemonstrateStudentActions(student);
                    break;
                default:
                    Console.WriteLine("Unknown person actions.");
                    break;
            }
        }

        private static void DemonstrateTeacherActions(ITeacherActions teacher)
        {
            Console.WriteLine("Demonstrating teacher actions.");
        }

        private static void DemonstrateStudentActions(IStudentActions student)
        {
            Console.WriteLine("Demonstrating student actions.");
        }

        public static void EnrollStudentInCourse(List<Student?> students, List<Course> courses, IUser user)
        {
            Exceptions.Expectations.CheckStudentsNotNull(students);
            Exceptions.Expectations.CheckCoursesNotNull(courses);
            
            if (user is not Teacher && user is not Admin)
            {
                Console.WriteLine("You do not have permission to enroll students in courses.");
                return;
            }

            try
            {
                Console.WriteLine("Select a student to enroll:");
                for (var i = 0; i < students.Count; i++)
                {
                    if (students[i] != null)
                    {
                        Console.WriteLine($"{i + 1}. {students[i].GetStudentFullName()}");
                    }
                }

                if (!int.TryParse(Console.ReadLine(), out var studentIndex) || studentIndex < 1 || studentIndex > students.Count)
                {
                    Console.WriteLine("Invalid student selection.");
                    return;
                }
                studentIndex--;

                Console.WriteLine("Select a course to enroll in:");
                for (var i = 0; i < courses.Count; i++)
                {
                    if (courses[i] != null)
                    {
                        Console.WriteLine($"{i + 1}. {courses[i].GetCourseName()}");
                    }
                }

                if (!int.TryParse(Console.ReadLine(), out var courseIndex) || courseIndex < 1 || courseIndex > courses.Count)
                {
                    Console.WriteLine("Invalid course selection.");
                    return;
                }
                courseIndex--;

                students[studentIndex]?.EnrollInCourse(courses[courseIndex]);
                Console.WriteLine($"{students[studentIndex]?.GetStudentFullName()} has been enrolled in {courses[courseIndex]?.GetCourseName()}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
