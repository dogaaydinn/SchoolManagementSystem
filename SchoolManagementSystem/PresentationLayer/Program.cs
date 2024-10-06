using SchoolManagementSystem.BusinessLogicLayer.Authentications;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.PresentationLayer.Handlers;
using SchoolManagementSystem.Interfaces.Actions;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers.ActionHandler;

namespace SchoolManagementSystem.PresentationLayer;

public static class Program
{
    public static void Main(string[] args)
    {
        var courses = DataProvider.GetCourses();
        var teachers = DataProvider.GetTeachers();
        var students = DataProvider.GetStudents();
        var admins = DataProvider.GetAdmins();
        var user = Authenticator.GetSchoolMember();
        if (user == null)
        {
            Console.WriteLine("Authentication failed. Exiting...");
            return;
        }

        ISchoolMemberActions? person = GetPerson(); 
        PersonHandler.DemonstrateActions(person, user);

        MenuHandler.DisplayMainMenu(students, courses, teachers, admins, user);
    }
    
    private static ISchoolMemberActions? GetPerson()
    {
        return new Student("John", "Doe", new DateTime(2000, 1, 1), 12345, 0, "hashedPassword");
    }
}