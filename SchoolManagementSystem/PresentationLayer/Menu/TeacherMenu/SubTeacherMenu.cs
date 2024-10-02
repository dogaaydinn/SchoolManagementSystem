using SchoolManagementSystem.PresentationLayer.Handlers;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.Interfaces.User;

namespace SchoolManagementSystem.PresentationLayer.Menu.TeacherMenu;

public class SubTeacherMenu
{
    public static void DisplaySubTeacherMenu(List<Teacher?>? teachers, object? user)
    {
        while (true)
        {
            Console.WriteLine("\nSub Teacher Menu:");
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
                    teacher = SchoolHandler.SelectTeacher(teachers);
                    if (teacher != null)
                    {
                        TeacherHandler.DisplayTeacherDetails(new List<Teacher?> { teacher }, user);
                    }
                    break;
                case "2":
                    // Implement Display Teacher Courses logic
                    break;
                case "3":
                   TeacherHandler.DisplayTeachersBySubject(teachers);
                    break;
                case "4":
                    TeacherHandler.DemonstrateTeacherMethods(teacher, user);
                    break;
                case "5":
                    teacher = SchoolHandler.SelectTeacher(teachers);
                    if (teacher != null)
                    {
                        TeacherHandler.GetTeacherById(teachers);
                    }
                    break;
                case "6":
                    teacher = SchoolHandler.SelectTeacher(teachers);
                    if (teacher != null)
                    {
                        TeacherHandler.GetTeacherByName(teachers);
                    }
                    break;
                case "7":
                    // Implement Get Teacher By Subject logic
                    break;
                case "8":
                    // Implement Get Teacher By Course logic
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
                case "13":
                    if (user != null)
                    {
                        TeacherHandler.AddNewTeacher(teachers, (IUser)user);
                    }
                    else
                    {
                        Console.WriteLine("Error: User is null.");
                    }
                    break;
                case "14":
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
                case "15":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}