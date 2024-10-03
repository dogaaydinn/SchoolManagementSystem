using SchoolManagementSystem.Interfaces;
using SchoolManagementSystem.Interfaces.Actions;

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