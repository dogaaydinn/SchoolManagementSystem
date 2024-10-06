using SchoolManagementSystem.Interfaces.Actions;
using SchoolManagementSystem.Interfaces.Helper;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers.ActionHandler;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.PresentationLayer.Helpers;

public class PersonHelper : IPersonHelper
{
    public void CheckAndDemonstrateActions(ISchoolMemberActions? person, object? user)
    {
        try
        {
            if (person == null)
                throw new ValidationException("No person provided.");

            if (user == null)
                throw new ValidationException("No user provided.");

            if (HasPermission(user, person))
                DemonstrateActions(person);
            else
                Console.WriteLine("You do not have permission to demonstrate these actions.");
        }
        catch (ValidationException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private static void DemonstrateActions(ISchoolMemberActions schoolMember)
    {
        try
        {
            switch (schoolMember)
            {
                case ITeacherActions teacher:
                    PersonActionDemonstrator.DemonstrateTeacherActions(teacher);
                    break;
                case IStudentActions student:
                    PersonActionDemonstrator.DemonstrateStudentActions(student);
                    break;
                default:
                    throw new ValidationException("Unknown member type.");
            }
        }
        catch (ValidationException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while demonstrating actions: {ex.Message}");
        }
    }

    private static bool HasPermission(object user, ISchoolMemberActions schoolMember)
    {
        try
        {
            return user switch
            {
                Admin => true,
                Teacher => schoolMember is ITeacherActions,
                Student => schoolMember is IStudentActions,
                _ => throw new ValidationException("User does not have permission.")
            };
        }
        catch (ValidationException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while checking permissions: {ex.Message}");
            return false;
        }
    }
}