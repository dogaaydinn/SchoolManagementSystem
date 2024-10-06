using SchoolManagementSystem.Interfaces.Actions;
using SchoolManagementSystem.Interfaces.Helper;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers.ActionHandler;

namespace SchoolManagementSystem.PresentationLayer.Helpers;

public class PersonHelper : IPersonHelper
{
    public void CheckAndDemonstrateActions(ISchoolMemberActions? person, object? user)
    {
        if (person == null)
        {
            Console.WriteLine("No person provided.");
            return;
        }

        if (user == null)
        {
            Console.WriteLine("No user provided.");
            return;
        }

        if (HasPermission(user, person))
            DemonstrateActions(person);
        else
            Console.WriteLine("You do not have permission to demonstrate these actions.");
    }

    private static void DemonstrateActions(ISchoolMemberActions schoolMember)
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
                Console.WriteLine("Unknown member type.");
                break;
        }
    }

    private static bool HasPermission(object user, ISchoolMemberActions schoolMember)
    {
        return user switch
        {
            Admin => true,
            Teacher => schoolMember is ITeacherActions,
            Student => schoolMember is IStudentActions,
            _ => false
        };
    }
}