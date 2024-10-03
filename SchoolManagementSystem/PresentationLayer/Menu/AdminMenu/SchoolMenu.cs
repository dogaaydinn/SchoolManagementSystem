using SchoolManagementSystem.BusinessLogicLayer.Validations;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;

namespace SchoolManagementSystem.PresentationLayer.Menu.AdminMenu
{
    public static class SchoolMenu
    {
        public static void DisplaySchoolMenu(List<Course>? courses, List<Student>? students, List<Teacher?>? teachers, IUser? user)
        {
            while (true)
            {
                Console.WriteLine("\nSchool Operations:");

                if (user == null)
                {
                    Console.WriteLine("Error: User is null.");
                    return;
                }

                switch (user)
                {
                    case Student student:
                        DisplayStudentMenu();
                        HandleStudentMenu(students, courses, student);
                        break;
                    case Teacher teacher:
                        DisplayTeacherMenu();
                        HandleTeacherMenu(teachers, courses, teacher);
                        break;
                    case Admin admin:
                        DisplayAdminMenu();
                        HandleAdminMenu(courses, students, teachers, admin);
                        break;
                    default:
                        Console.WriteLine("Unknown user type.");
                        return;
                }
            }
        }

        private static void HandleStudentMenu(List<Student?>? students, List<Course>? courses, Student? student)
        {
            var choice = GetUserChoice();

            if (choice == null) return; // Early exit if choice is invalid

            switch (choice)
            {
                case "1":
                    SchoolHandler.DisplayAllDetails(courses, students, new List<Teacher?>(), student);
                    break;
                case "2":
                    if (courses == null)
                    {
                        Console.WriteLine("Courses list is null. Cannot enroll student in course.");
                    }
                    else
                    {
                        SchoolHandler.EnrollStudentInCourse(students.ToList(), courses, student);
                    }
                    break;
                case "3":
                    // Add code for viewing grades here
                    break;
                case "4":
                    return; // Back to main menu
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        private static void HandleAdminMenu(List<Course>? courses, List<Student?>? students, List<Teacher?>? teachers, Admin admin)
        {
            var choice = GetUserChoice();

            if (choice == null) return; // Early exit if choice is invalid

            switch (choice)
            {
                case "1":
                    SchoolHandler.DisplayAllDetails(courses, students.ToList(), teachers, admin);
                    break;
                case "2":
                    // Add code for course management here
                    break;
                case "3":
                    // Add code for student and teacher management here
                    break;
                case "4":
                    if (courses == null)
                    {
                        Console.WriteLine("Courses list is null. Cannot record grades for students.");
                    }
                    else
                    {
                        SchoolHandler.RecordGradesForStudents(courses, teachers);
                    }
                    break;
                case "5":
                    return; // Back to main menu
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        private static void HandleTeacherMenu(List<Teacher?>? teachers, List<Course>? courses, Teacher? teacher)
        {
            var choice = GetUserChoice();

            if (choice == null) return; // Early exit if choice is invalid

            switch (choice)
            {
                case "1":
                    TeacherHandler.DisplayTeacherDetails(new List<Teacher?> { teacher }, teachers);
                    break;
                case "2":
                    TeacherHandler.UpdateTeacherId(teachers, teacher);
                    break;
                case "3":
                    return; // Back to main menu
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        // Get the user's choice and validate it
        private static string? GetUserChoice()
        {
            Console.Write("Enter your choice: ");
            var choice = Console.ReadLine();

            if (string.IsNullOrEmpty(choice))
            {
                Console.WriteLine("Choice cannot be null or empty.");
                return null;
            }

            return choice;
        }

        // Display menu options for students
        private static void DisplayStudentMenu()
        {
            Console.WriteLine("1. View All Details");
            Console.WriteLine("2. Enroll in Course");
            Console.WriteLine("3. View Grades");
            Console.WriteLine("4. Back to Main Menu");
        }

        // Display menu options for teachers
        private static void DisplayTeacherMenu()
        {
            Console.WriteLine("1. View All Details");
            Console.WriteLine("2. View Student Details");
            Console.WriteLine("3. Assign Courses to Students");
            Console.WriteLine("4. Record Grades for Students");
            Console.WriteLine("5. Back to Main Menu");
        }

        // Display menu options for admins
        private static void DisplayAdminMenu()
        {
            Console.WriteLine("1. View All Details");
            Console.WriteLine("2. Manage Courses");
            Console.WriteLine("3. Manage Students and Teachers");
            Console.WriteLine("4. Record Grades for Students");
            Console.WriteLine("5. Back to Main Menu");
        }
    }
}
