using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;

namespace SchoolManagementSystem.PresentationLayer.Menu.AdminMenu;

public static class SchoolMenu
{
    public static void DisplaySchoolMenu(List<Course>? courses, List<Student?>? students, List<Teacher?>? teachers, IUser? user)
    {
        while (true)
        {
            Console.WriteLine("\nSchool Operations:");

            switch (user)
            {
                case Student student:
                    if (courses == null)
                    {
                        Console.WriteLine("Courses list is null. Cannot display student menu.");
                        return;
                    }
                    StudentMenu.StudentMenu.DisplayStudentMenu(students, courses, student);
                    break;
                case Teacher teacher:
                    TeacherMenu.TeacherMenu.DisplayTeacherMenu(teachers, students.Cast<Student>().ToList(), courses, teacher);
                    break;
                case Admin:
                    AdminMenu.DisplayAdminMenu(courses, students, teachers, user);
                    break;
                default:
                    Console.WriteLine("Unknown user type.");
                    return;
            }

            Console.Write("Enter your choice: ");
            var choice = Console.ReadLine();

            if (string.IsNullOrEmpty(choice))
            {
                Console.WriteLine("Choice cannot be null or empty.");
                continue;
            }

            switch (user)
            {
                case Student student:
                    HandleStudentMenu(choice, courses, students, student);
                    break;

                case Teacher teacher:
                    HandleTeacherMenu(choice, courses, teachers, teacher);
                    break;

                case Admin:
                    HandleAdminMenu(choice, courses, students, teachers, user);
                    break;
            }
        }
    }
    private static void HandleCourseMenu(string choice, List<Course>? courses, Course course, IUser? user)
    {
        if (courses == null)
        {
            Console.WriteLine("Courses list is null. Cannot proceed.");
            return;
        }

        switch (choice)
        {
            case "1":
                CourseHandler.DisplayCourseDetails(new List<Course> { course }, user);
                break;
            case "2":
                CourseHandler.UpdateCourseId(course, user);
                break;
            case "3":
                CourseHandler.UpdateCourseName(course, user);
                break;
            case "4":
                return; 
            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }
    private static void HandleStudentMenu(string choice, List<Course>? courses, List<Student?>? students, Student? student)
    {
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
                return; 
            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }

    private static void HandleAdminMenu(string choice, List<Course>? courses, List<Student?>? students, List<Teacher?>? teachers, IUser? user)
    {
        switch (choice)
        {
            case "1":
                SchoolHandler.DisplayAllDetails(courses, students.ToList(), teachers, user);
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
                return; 
            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }
    
    private static void HandleTeacherMenu(string choice, List<Course>? courses, List<Teacher?>? teachers, Teacher? teacher)
    {
        switch (choice)
        {
            case "1":
                TeacherHandler.DisplayTeacherDetails(new List<Teacher?> { teacher }, teachers);
                break;
            case "2":
                TeacherHandler.UpdateTeacherId(teachers, teacher);
                break;
            case "3":
                return; 
            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }

    private static void DisplayStudentMenu()
    {
        Console.WriteLine("1. View All Details");
        Console.WriteLine("2. Enroll in Course");
        Console.WriteLine("3. View Grades");
        Console.WriteLine("4. Back to Main Menu");
    }

    private static void DisplayTeacherMenu()
    {
        Console.WriteLine("1. View All Details");
        Console.WriteLine("2. View Student Details");
        Console.WriteLine("3. Assign Courses to Students");
        Console.WriteLine("4. Record Grades for Students");
        Console.WriteLine("5. Back to Main Menu");
    }
    
}