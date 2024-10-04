using SchoolManagementSystem.BusinessLogicLayer.Validations;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;

namespace SchoolManagementSystem.PresentationLayer.Menu.AdminMenu;

public static class AdminTeacherMenu
{
    public static void DisplayTeacherMenu(List<Teacher?> teachers, List<Course> courses, object user)
    {
        while (true)
        {
            DisplayMenuOptions();
            var choice = Console.ReadLine();

            if (!ValidationHelper.ValidateUserInput(choice, out var validChoice)) continue;
            Teacher teacher = null;

            switch (validChoice)
            {
                case 1:
                    if (teacher != null)
                    {
                        TeacherHandler.DisplayTeacherDetails(new List<Teacher?> { teacher }, user);
                    }
                    else
                    {
                        Console.WriteLine("No teacher selected. Please select a teacher first.");
                    }
                    break;
                case 2:
                    TeacherHandler.DisplayTeacherNames(teachers);
                    break;
                case 3:
                    TeacherHandler.GetTeacherById(teachers);
                    break;
                case 4:
                    TeacherHandler.GetTeacherByName(teachers);
                    break;
                case 5:
                    TeacherHandler.DisplayTeachersBySubject(teachers);
                    break;
                case 6:
                    TeacherHandler.DisplayAllTeachers(teachers);
                    break;
                case 7:
                    TeacherHandler.UpdateTeacherSubject(teachers, user);
                    break;
                case 8:
                    teacher = TeacherHandler.GetTeacherById(teachers);
                    if (teacher != null)
                    {
                        TeacherHandler.UpdateTeacherName(teacher, user);
                    }
                    else
                    {
                        Console.WriteLine("Teacher not found.");
                    }
                    break;
                case 9:
                    TeacherHandler.AddNewTeacher(teachers, user);
                    break;
                case 10:
                    TeacherHandler.RemoveTeacher(teachers, user);
                    break;
                case 11:
                    return; 
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
    
    private static void DisplayMenuOptions()
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
    }
}