using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;

namespace SchoolManagementSystem.PresentationLayer.Menu.TeacherMenu;

public class TeacherMenu
{
    public static void DisplayTeacherMenu(List<Teacher> teachers, List<Student> students, List<Course> courses, object? user)
    {
        while (true)
        {
            Console.WriteLine("\nTeacher Menu:");
            Console.WriteLine("1. Display Teacher Details");
            Console.WriteLine("2. Manage Students");
            Console.WriteLine("3. Manage Courses");
            Console.WriteLine("4. Display Teacher Names");
            Console.WriteLine("5. Display Teachers By Subject");
            Console.WriteLine("6. Display All Teachers");
            Console.WriteLine("7. Add New Teacher");
            Console.WriteLine("8. Remove Teacher");
            Console.WriteLine("9. Exit");
            Console.Write("Enter your choice: ");

            var choice = Console.ReadLine();

            if (string.IsNullOrEmpty(choice))
            {
                Console.WriteLine("Input cannot be empty. Please try again.");
                continue;
            }

            switch (choice)
            {
                case "1":
                    var teacher = SelectTeacher(teachers);
                    if (teacher != null)
                    {
                        TeacherHandler.DisplayTeacherDetails(new List<Teacher?> { teacher }, user);
                    }
                    break;
                case "2":
                    SubStudentMenu.DisplayStudentMenu(students, user);
                    break;
                case "3":
                    SubCourseMenu.DisplayCourseMenu(courses, students, user);
                    break;
                case "4":
                    TeacherHandler.DisplayTeacherNames(teachers);
                    break;
                case "5":
                    TeacherHandler.DisplayTeachersBySubject(teachers);
                    break;
                case "6":
                    TeacherHandler.DisplayAllTeachers(teachers);
                    break;
                case "7":
                    if (user != null)
                    {
                        TeacherHandler.AddNewTeacher(teachers, (IUser)user);
                    }
                    else
                    {
                        Console.WriteLine("Error: User is null.");
                    }
                    break;
                case "8":
                    var teacherToRemove = SelectTeacher(teachers);
                    if (teacherToRemove != null)
                    {
                        if (user != null)
                        {
                            TeacherHandler.RemoveTeacher(teachers, teacherToRemove, (IUser)user);
                        }
                        else
                        {
                            Console.WriteLine("Error: User is null.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: Teacher is null.");
                    }
                    break;
                case "9":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    private static Teacher? SelectTeacher(List<Teacher> teachers)
    {
        if (teachers == null || teachers.Count == 0)
        {
            Console.WriteLine("No teachers available.");
            return null;
        }

        Console.WriteLine("Select a teacher:");
        for (var i = 0; i < teachers.Count; i++)
        {
            var teacher = teachers[i];
            Console.WriteLine($"{i + 1}. {teacher.GetTeacherFullName()} (ID: {teacher.GetTeacherId()})");
        }

        if (int.TryParse(Console.ReadLine(), out var teacherIndex) && teacherIndex >= 1 && teacherIndex <= teachers.Count)
        {
            return teachers[teacherIndex - 1];
        }

        Console.WriteLine("Invalid teacher selection.");
        return null;
    }
}