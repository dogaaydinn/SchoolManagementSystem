using SchoolManagementSystem.BusinessLogicLayer.Exceptions;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.PresentationLayer.Handlers
{
    public static class StudentHandler
    {
        public static void AssignGradeToStudent(List<Course>? courses, List<Student?> students)
        {
            // Check for null values before proceeding
            Exceptions.Expectations.CheckCoursesNotNull(courses);
            Exceptions.Expectations.CheckStudentsNotNull(students);

            Course? course = GetCourseByIdOrName(courses);
            if (course == null) return;

            var student = GetStudentById(students);
            if (student == null) return;

            Console.WriteLine("Enter the grade to assign:");
            if (!double.TryParse(Console.ReadLine(), out var gradeValue) || gradeValue < 0 || gradeValue > 100)
            {
                Console.WriteLine("Invalid grade. Please enter a value between 0 and 100.");
                return;
            }

            if (course.IsStudentEnrolled(student))
            {
                course.AssignGrade(student, gradeValue);
                Console.WriteLine($"Assigned grade {gradeValue} to {student.GetStudentFullName()} for {course.GetCourseName()}.");
            }
            else
            {
                Console.WriteLine($"Student {student.GetStudentFullName()} is not enrolled in course {course.GetCourseName()}.");
            }
        }

        private static Course? GetCourseByIdOrName(List<Course>? courses)
        {
            while (true)
            {
                Console.WriteLine("Enter the course ID to assign a grade:");
                if (int.TryParse(Console.ReadLine(), out var courseId))
                {
                    var course = courses?.Find(c => c.GetCourseId() == courseId);
                    if (course != null) return course;

                    Console.WriteLine("Course not found. Enter the course name:");
                    var courseName = Console.ReadLine();
                    course = courses?.Find(c => c.GetCourseName().Equals(courseName, StringComparison.OrdinalIgnoreCase));
                    if (course != null) return course;

                    Console.WriteLine("Course not found. Would you like to see the list of courses? (yes/no)");
                    var userResponse = Console.ReadLine()?.Trim().ToLower();
                    if (userResponse == "yes")
                    {
                        DisplayCourseNames(courses);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid course ID. Please try again.");
                }
            }
        }

        private static Student? GetStudentById(List<Student?> students)
        {
            Console.WriteLine("Enter the student ID to assign a grade:");
            if (int.TryParse(Console.ReadLine(), out var studentId))
            {
                var student = students?.Find(s => s != null && s.GetStudentId() == studentId);
                if (student != null) return student;

                Console.WriteLine("Student not found. Please try again.");
                return null;
            }
            else
            {
                Console.WriteLine("Invalid student ID. Please try again.");
                return null;
            }
        }

        private static void DisplayCourseNames(List<Course>? courses)
        {
            Console.WriteLine("Course Names:");
            if (courses == null) return;
            foreach (var course in courses)
            {
                Console.WriteLine($"Course ID: {course.GetCourseId()}, Course Name: {course.GetCourseName()}");
            }
        }

        public static void DisplayStudentDetails(List<Student?> students, List<Course>? courses)
        {
            Exceptions.Expectations.CheckStudentsNotNull(students);
            Exceptions.Expectations.CheckCoursesNotNull(courses);

            Console.WriteLine("Do you want to search by:");
            Console.WriteLine("1. Student ID");
            Console.WriteLine("2. Student Name");
            Console.WriteLine("3. List all students");
            Console.WriteLine("4. Students in a specific course");
            Console.Write("Enter your choice (1, 2, 3, or 4): ");

            if (!int.TryParse(Console.ReadLine(), out var choice) || choice < 1 || choice > 4)
            {
                Console.WriteLine("Invalid choice. Please select 1, 2, 3, or 4.");
                return;
            }

            switch (choice)
            {
                case 1:
                    var studentById = GetStudentById(students);
                    if (studentById != null)
                    {
                        DisplayStudentActions(studentById, courses);
                    }
                    break;

                case 2:
                    var studentByName = GetStudentByName(students);
                    if (studentByName != null)
                    {
                        DisplayStudentActions(studentByName, courses);
                    }
                    break;

                case 3:
                    DisplayStudentNamesList(students);
                    break;

                case 4:
                    DisplayStudentsInCourse(courses);
                    break;
            }
        }

        private static Student? GetStudentByName(List<Student?> students)
        {
            Console.WriteLine("Enter student name to display details:");
            var studentName = Console.ReadLine();
            var student = students?.Find(s => s != null && s.GetStudentFullName().Equals(studentName, StringComparison.OrdinalIgnoreCase));
            if (student == null)
            {
                Console.WriteLine("Student not found.");
            }
            return student;
        }

        private static void DisplayStudentsInCourse(List<Course>? courses)
        {
            Console.WriteLine("Enter course ID to list students:");
            if (int.TryParse(Console.ReadLine(), out var courseId))
            {
                var course = courses?.Find(c => c.GetCourseId() == courseId);
                if (course != null)
                {
                    var enrolledStudents = course.GetEnrolledStudents();
                    DisplayStudentNamesList(enrolledStudents);
                }
                else
                {
                    Console.WriteLine("Course not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid course ID.");
            }
        }

        private static void DisplayStudentNamesList(List<Student?> students)
        {
            Exceptions.Expectations.CheckStudentsNotNull(students);

            Console.WriteLine("Student Names:");
            for (var i = 0; i < students.Count; i++)
            {
                var student = students[i];
                if (student != null)
                {
                    Console.WriteLine($"{i + 1}. {student.GetStudentFullName()}");
                }
            }

            Console.WriteLine("Enter the number of the student to view details or perform actions, or type 'exit' to return to the main menu:");
            var input = Console.ReadLine()?.Trim().ToLower();

            if (input == "exit") return;

            if (int.TryParse(input, out var studentIndex) && studentIndex > 0 && studentIndex <= students.Count)
            {
                var selectedStudent = students[studentIndex - 1];
                DisplayStudentActions(selectedStudent, null);
            }
            else
            {
                Console.WriteLine("Invalid input. Please try again.");
            }
        }

        private static void DisplayStudentActions(Student? student, List<Course>? courses)
        {
            Exceptions.Expectations.CheckStudentNotNull(student);

            if (student == null) return;
            Console.WriteLine($"Actions for {student.GetStudentFullName()} (ID: {student.GetStudentId()}):");
            Console.WriteLine("1. View grades");
            Console.WriteLine("2. Update GPA");
            Console.WriteLine("3. Exit");
            Console.Write("Enter your choice: ");

            if (int.TryParse(Console.ReadLine(), out var choice))
            {
                switch (choice)
                {
                    case 1:
                        DisplayStudentGrades(student);
                        break;

                    case 2:
                        UpdateStudentGpa(student);
                        break;

                    case 3:
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please try again.");
            }
        }

        private static void DisplayStudentGrades(Student student)
        {
            Console.WriteLine($"Grades for {student.GetStudentFullName()}:");
            foreach (var course in student.GetEnrolledCourses())
            {
                var grade = student.GetAssignedGrades(course);
                Console.WriteLine($"Course: {course.GetCourseName()}, Grade: {grade}");
            }
        }

        private static void UpdateStudentGpa(Student student)
        {
            Exceptions.Expectations.CheckStudentNotNull(student);

            Console.WriteLine($"Current GPA: {student.GetGpa()}");
            Console.WriteLine("Enter new GPA (or type 'exit' to cancel):");

            var input = Console.ReadLine()?.Trim().ToLower();
            if (input == "exit") return;

            if (double.TryParse(input, out var newGpa) && newGpa is >= 0 and <= 4)
            {
                student.SetGpa(newGpa);
                Console.WriteLine($"Updated GPA for {student.GetStudentFullName()} to: {newGpa}");
            }
            else
            {
                Console.WriteLine("Invalid GPA. Please enter a value between 0 and 4.");
            }
        }
    }
}
