using SchoolManagementSystem.BusinessLogicLayer.Exceptions;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;
using SchoolManagementSystem.PresentationLayer.Helpers;

namespace SchoolManagementSystem.PresentationLayer.Menu.TeacherMenu;

public static class SubTeacherMenu
{
    public static void DisplayTeacherMenu(List<Teacher?> teachers, object user)
    {
        while (true)
        {
            Console.WriteLine("\nSub Teacher Menu:");
            DisplayMenuOptions();
            var choice = Console.ReadLine();

            if (string.IsNullOrEmpty(choice))
            {
                Console.WriteLine("Input cannot be empty. Please try again.");
                continue;
            }

            if (!HandleTeacherMenuChoice(choice, teachers, user)) return;
        }
    }

    private static void DisplayMenuOptions()
    {
        Console.WriteLine("1. Display Teacher Details");
        Console.WriteLine("2. Display Teacher Courses");
        Console.WriteLine("3. Display Teacher By Subject");
        Console.WriteLine("4. Demonstrate Teacher Methods");
        Console.WriteLine("5. Get Teacher By ID");
        Console.WriteLine("6. Get Teacher By Name");
        Console.WriteLine("7. Get Teacher By Subject");
        Console.WriteLine("8. Get Teacher By Course");
        Console.WriteLine("9. Display Teacher Names");
        Console.WriteLine("10. Display Teachers By Subject");
        Console.WriteLine("11. Display All Teachers");
        Console.WriteLine("12. Add New Teacher");
        Console.WriteLine("13. Remove Teacher");
        Console.WriteLine("14. Exit");
        Console.Write("Enter your choice: ");
    }

    private static bool HandleTeacherMenuChoice(string choice, List<Teacher?> teachers, object user)
    {
        var schoolHelper = new SchoolHelper();
        var teacher = SchoolHelper.SelectTeacher(teachers);
        switch (choice)
        {
            case "1":
                if (teacher != null) TeacherHandler.DisplayTeacherDetails(new List<Teacher?> { teacher }, user);
                break;
            case "2":
                TeacherHandler.DisplayTeacherCourses(teachers, user);
                break;
            case "3":
                TeacherHandler.DisplayTeachersBySubject(teachers);
                break;
            case "4":
                try
                {
                    if (teacher != null)
                        SchoolHandler.DemonstrateActions(teachers, user);
                    else
                        throw new Exceptions.TeacherNotFoundException("No teacher selected. Please select a teacher first.");
                }
                catch (Exceptions.TeacherNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                break;
            case "5":
                if (teacher != null) TeacherHandler.GetTeacherById(teachers);
                break;
            case "6":
                if (teacher != null) TeacherHandler.GetTeacherByName(teachers);
                break;
            case "8":
                TeacherHelper.DisplayTeachersBySubject(teachers);
                break;
            case "9":
                TeacherHandler.DisplayTeacherNames(teachers);
                break;
            case "10":
                TeacherHandler.DisplayTeachersBySubject(teachers);
                break;
            case "11":
                TeacherHandler.DisplayAllTeachers(teachers);
                break;
            case "12":
                try
                {
                    if (user != null)
                        TeacherHelper.AddNewTeacher(teachers, (IUser)user);
                    else
                        throw new Exceptions.UserNotFoundException();
                }
                catch (Exceptions.UserNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                break;
            case "13":
                try
                {
                    if (teacher != null && user != null)
                        TeacherHandler.RemoveTeacher(teachers, teacher, (IUser)user);
                    else if (teacher == null)
                        throw new Exceptions.TeacherNotFoundException("Error: Teacher is null.");
                    else
                        throw new Exceptions.UserNotFoundException();
                }
                catch (Exceptions.TeacherNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exceptions.UserNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                break;
            case "14":
                return false;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }

        return true;
    }
}