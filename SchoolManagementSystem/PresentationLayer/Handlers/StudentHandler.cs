using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.PresentationLayer.Handlers
{
    public static class StudentHandler
    {
        public static void AssignGradeToStudent(List<Course>? courses, List<Student?> students)
        {
            Course? course = null;

            while (course == null)
            {
                Console.WriteLine("Enter the course ID to assign a grade:");
                if (int.TryParse(Console.ReadLine(), out var courseId))
                {
                    course = courses?.Find(c => c.GetCourseId() == courseId);
                    if (course != null) continue;

                    Console.WriteLine("Course not found. Enter the course name:");
                    var courseName = Console.ReadLine();
                    course = courses?.Find(c => c.GetCourseName().Equals(courseName, StringComparison.OrdinalIgnoreCase));
                    if (course != null) continue;

                    Console.WriteLine("Course not found. Would you like to see the list of courses? (yes/no)");
                    var userResponse = Console.ReadLine()?.Trim().ToLower();
                    if (userResponse == "yes")
                    {
                        DisplayCourseNames(courses);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid course ID. Please try again.");
                }
            }

            Console.WriteLine("Enter the student ID to assign a grade:");
            if (!int.TryParse(Console.ReadLine(), out var studentId))
            {
                Console.WriteLine("Invalid student ID. Please try again.");
                return;
            }

            var student = students.Find(s => s != null && s.GetStudentId() == studentId);
            if (student == null)
            {
                Console.WriteLine("Student not found. Please try again.");
                return;
            }

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

        private static void DisplayCourseNames(List<Course>? courses)
        {
            Console.WriteLine("Course Names:");
            if (courses != null)
                foreach (var course in courses)
                {
                    Console.WriteLine($"Course ID: {course.GetCourseId()}, Course Name: {course.GetCourseName()}");
                }
        }

        public static void DisplayStudentDetails(List<Student> students, List<Course>? courses)
        {
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

            Student? student = null;

            switch (choice)
            {
                case 1:
                    Console.WriteLine("Enter student ID to display details:");
                    if (int.TryParse(Console.ReadLine(), out var studentId))
                        student = students.Find(s => s.GetStudentId() == studentId);
                    else
                        Console.WriteLine("Invalid student ID.");
                    break;

                case 2:
                    Console.WriteLine("Enter student name to display details:");
                    var studentName = Console.ReadLine();
                    student = students.Find(s => s.GetStudentFullName().Equals(studentName, StringComparison.OrdinalIgnoreCase));
                    break;

                case 3:
                    DisplayStudentNames(students, courses);
                    return;

                case 4:
                    Console.WriteLine("Enter course ID to list students:");
                    if (int.TryParse(Console.ReadLine(), out var courseId))
                    {
                        var course = courses.Find(c => c.GetCourseId() == courseId);
                        if (course != null)
                        {
                            var enrolledStudents = course.GetEnrolledStudents();
                            DisplayStudentNames(enrolledStudents, courses);
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
                    return;
            }

            if (student != null)
            {
                Console.WriteLine(
                    $"Student ID: {student.GetStudentId()}, Name: {student.GetStudentFullName()}, GPA: {student.GetGpa()}");
            }
            else
            {
                Console.WriteLine("Student not found. Would you like to see the list of students? (yes/no)");
                var userResponse = Console.ReadLine()?.Trim().ToLower();

                if (userResponse == "yes") DisplayStudentNames(students, courses);
            }
        }

        private static void DisplayStudentNames(List<Student> students, List<Course>? courses)
        {
            while (true)
            {
                Console.WriteLine("Do you want to search by:");
                Console.WriteLine("1. Student ID");
                Console.WriteLine("2. Student Name");
                Console.WriteLine("3. List all students");
                Console.WriteLine("4. Students in a specific course");
                Console.Write("Enter your choice (1, 2, 3, or 4): ");

                if (!int.TryParse(Console.ReadLine(), out var choice) || choice < 1 || choice > 4)
                {
                    Console.WriteLine("Invalid choice. Please select 1, 2, 3, or 4.");
                    continue;
                }

                Student? student = null;

                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Enter student ID to display details:");
                        if (int.TryParse(Console.ReadLine(), out var studentId))
                            student = students.Find(s => s!.GetStudentId() == studentId);
                        else
                            Console.WriteLine("Invalid student ID.");
                        break;

                    case 2:
                        Console.WriteLine("Enter student name to display details:");
                        var studentName = Console.ReadLine();
                        student = students.Find(s => s!.GetStudentFullName().Equals(studentName, StringComparison.OrdinalIgnoreCase));
                        break;

                    case 3:
                        DisplayStudentNamesList(students);
                        return;

                    case 4:
                        Console.WriteLine("Enter course ID to list students:");
                        if (int.TryParse(Console.ReadLine(), out var courseId))
                        {
                            var course = courses.Find(c => c.GetCourseId() == courseId);
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
                        return;
                }

                if (student != null)
                {
                    DisplayStudentActions(student, courses);
                }
                else
                {
                    Console.WriteLine("Student not found. Would you like to see the list of students? (yes/no)");
                    var userResponse = Console.ReadLine()?.Trim().ToLower();

                    if (userResponse == "yes") DisplayStudentNamesList(students);
                }
            }
        }

        private static void DisplayStudentNamesList(List<Student> students)
        {
            Console.WriteLine("Student Names:");
            for (var i = 0; i < students.Count; i++) Console.WriteLine($"{i + 1}. {students[i]!.GetStudentFullName()}");

            Console.WriteLine(
                "Enter the number of the student to view details or perform actions, or type 'exit' to return to the main menu:");
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

        public static void UpdateStudentId(List<Student> students)
        {
            while (true)
            {
                Console.WriteLine("Enter the current student ID (or type 'exit' to cancel):");
                var input = Console.ReadLine()?.Trim().ToLower();
                if (input == "exit") return;

                if (int.TryParse(input, out var currentStudentId))
                {
                    var student = students.FirstOrDefault(s => s.GetStudentId() == currentStudentId);
                    if (student == null)
                    {
                        Console.WriteLine("Student not found. Please try again.");
                        continue;
                    }

                    while (true)
                    {
                        Console.WriteLine($"Current Student: {student.GetStudentFullName()}");
                        Console.WriteLine("Enter the new student ID (or type 'exit' to cancel):");
                        var newInput = Console.ReadLine()?.Trim().ToLower();
                        if (newInput == "exit") return;

                        if (int.TryParse(newInput, out var newStudentId) && newStudentId != currentStudentId)
                        {
                            student.SetStudentId(newStudentId);
                            Console.WriteLine($"Updated Student ID to: {newStudentId}");
                            return;
                        }
                        else
                        {
                            Console.WriteLine("Invalid input or same as current ID. Please try again.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid student ID. Please try again.");
                }
            }
        }

        private static void DisplayStudentActions(Student student, List<Course>? courses)
        {
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
                        Console.WriteLine($"Grades for {student.GetStudentFullName()}:");
                        foreach (var course in student.GetEnrolledCourses())
                        {
                            var grade = student.GetAssignedGrades(course);
                            Console.WriteLine($"Course: {course.GetCourseName()}, Grade: {grade}");
                        }
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

        private static void UpdateStudentGpa(Student student)
        {
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
