using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;

namespace SchoolManagementSystem.PresentationLayer.Menu;

public static class SchoolMenu
{
    public static void DisplaySchoolMenu(List<Course>? courses, List<Student> students, List<Teacher> teachers, IUser user)
    {
        while (true)
        {
            Console.WriteLine("\nSchool Operations:");
            
            if (user is Student)
            {
                StudentMenu.DisplayStudentMenu();
            }
            else if (user is Teacher)
            {
                TeacherMenu.DisplayTeacherMenu();
            }
            else if (user is Admin)
            {
                DisplayAdminMenu(); 
            }
            else
            {
                Console.WriteLine("Unknown user type.");
                return; 
            }

            Console.Write("Enter your choice: ");
            var choice = Console.ReadLine();

            switch (user)
            {
                case Student:
                    HandleStudentMenu(choice, courses, students);
                    break;

                case Teacher:
                    HandleTeacherMenu(choice, courses, students);
                    break;

                case Admin:
                    HandleAdminMenu(choice, courses, students, teachers);
                    break;
            }
        }
    }

    private static void DisplayAdminMenu()
    {
        Console.WriteLine("1. View All Details");
        Console.WriteLine("2. Manage Courses");
        Console.WriteLine("3. Manage Students and Teachers");
        Console.WriteLine("4. Record Grades for Students");
        Console.WriteLine("5. Back to Main Menu");
    }

    private static void HandleAdminMenu(string choice, List<Course>? courses, List<Student> students, List<Teacher> teachers)
    {
        switch (choice)
        {
            case "1":
                SchoolHandler.DisplayAllDetails(courses, students, teachers);
                break;
            case "2":
                // Kurs yönetimi işlemleri için gerekli kod buraya eklenmeli
                break;
            case "3":
                // Öğrenci ve öğretmen yönetimi işlemleri için gerekli kod buraya eklenmeli
                break;
            case "4":
                SchoolHandler.RecordGradesForStudents(courses);
                break;
            case "5":
                return; // Ana Menüye Dön
            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }

    private static void HandleStudentMenu(string choice, List<Course>? courses, List<Student> students)
    {
        switch (choice)
        {
            case "1":
                SchoolHandler.DisplayAllDetails(courses, students, null);
                break;
            case "2":
                SchoolHandler.EnrollStudentInCourse(students, courses);
                break;
            case "3":
                // Not görüntüleme işlemi için gerekli kod buraya eklenmeli
                break;
            case "4":
                return; // Ana Menüye Dön
            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }

    private static void HandleTeacherMenu(string choice, List<Course>? courses, List<Student> students)
    {
        switch (choice)
        {
            case "1":
                SchoolHandler.DisplayAllDetails(courses, students, null);
                break;
            case "2":
                SchoolHandler.DisplayStudentDetails(students, courses);
                break;
            case "3":
                SchoolHandler.AssignCoursesToStudents(courses, students);
                break;
            case "4":
                SchoolHandler.RecordGradesForStudents(courses);
                break;
            case "5":
                return; // Ana Menüye Dön
            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }

    private static void DemonstratePersonActions(List<Student> students, List<Teacher> teachers, List<Course>? courses)
    {
        Console.WriteLine("Enter the person type (student/teacher/course):");
        var personType = Console.ReadLine()?.ToLower();

        switch (personType)
        {
            case "student":
                if (students.Count > 0)
                {
                    SchoolHandler.DemonstrateActions(students[0]);
                }
                else
                {
                    Console.WriteLine("No students available.");
                }
                break;
            case "teacher":
                if (teachers.Count > 0)
                {
                    SchoolHandler.DemonstrateActions(teachers[0]);
                }
                else
                {
                    Console.WriteLine("No teachers available.");
                }
                break;
            case "course":
                if (courses != null && courses.Count > 0)
                {
                    SchoolHandler.DemonstrateActions(courses[0]);
                }
                else
                {
                    Console.WriteLine("No courses available.");
                }
                break;
            default:
                Console.WriteLine("Invalid person type.");
                break;
        }
    }
}
