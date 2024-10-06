using SchoolManagementSystem.Interfaces.Actions;
using SchoolManagementSystem.PresentationLayer.Helpers;

namespace SchoolManagementSystem.PresentationLayer.Handlers;

public static class PersonHandler
{
    public static void DemonstrateActions(ISchoolMemberActions? person, object? user)
    {
        if (person == null)
        {
            Console.WriteLine("No person provided.");
            return;
        }

        var personHelper = new PersonHelper();
        personHelper.CheckAndDemonstrateActions(person, user);
    }
}