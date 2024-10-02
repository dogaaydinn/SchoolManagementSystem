using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;

namespace SchoolManagementSystem.PresentationLayer.Menu.AdminMenu;

public static class AdminTeacherMenu
{
    public static void DisplayTeacherMenu(List<Teacher?>? teachers, List<Course> courses, object? user)
{
    while (true)
    {
        Console.WriteLine("\nAdmin Teacher Menu:");
        Console.WriteLine("1. Display Teacher Details");
        Console.WriteLine("2. Display Teacher Names");
        Console.WriteLine("3. Get Teacher By ID");
        Console.WriteLine("4. Get Teacher By Name");
        Console.WriteLine("5. Display Teachers By Subject");
        Console.WriteLine("6. Display All Teachers");
        Console.WriteLine("7. Update Teacher Subject");
        Console.WriteLine("8. Update Teacher Name");
        Console.WriteLine("9. Add New Teacher");
        Console.WriteLine("10. Remove Teacher");
        Console.WriteLine("11. Exit");
        Console.Write("Enter your choice: ");

        var choice = Console.ReadLine();

        if (string.IsNullOrEmpty(choice))
        {
            Console.WriteLine("Input cannot be empty. Please try again.");
            continue;
        }
        Teacher? teacher = null;

        switch (choice)
        {
            case "1":
                if (teacher != null)
                {
                    TeacherHandler.DisplayTeacherDetails(new List<Teacher?> { teacher }, user);
                }
                break;
            case "2":
                TeacherHandler.DisplayTeacherNames(teachers);
                break;
            case "3":
                teacher = SchoolHandler.SelectTeacher(teachers);
                if (teacher != null)
                {
                    TeacherHandler.GetTeacherById(teachers);
                }
                break;
            case "4":
                teacher = SchoolHandler.SelectTeacher(teachers);
                if (teacher != null)
                {
                    TeacherHandler.GetTeacherByName(teachers);
                }
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
                    TeacherHandler.UpdateTeacherSubject(teachers, user);
                }
                else
                {
                    Console.WriteLine("Error: User is null.");
                }
                break;
            case "8":
                teacher = SchoolHandler.SelectTeacher(teachers);
                if (teacher != null)
                {
                    if (user != null)
                    {
                        TeacherHandler.UpdateTeacherName(teacher, (IUser)user);
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
                if (user != null)
                {
                    TeacherHandler.AddNewTeacher(teachers, (IUser)user);
                }
                else
                {
                    Console.WriteLine("Error: User is null.");
                }
                break;
            case "10":
                teacher = SchoolHandler.SelectTeacher(teachers);
                if (teacher != null)
                {
                    if (user != null)
                    {
                        TeacherHandler.RemoveTeacher(teachers, teacher, (IUser)user);
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
            case "11":
                return;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }
}
}