using SchoolManagementSystem.BusinessLogicLayer.Exceptions;
using SchoolManagementSystem.Interfaces.Actions;
using SchoolManagementSystem.PresentationLayer.Helpers;

namespace SchoolManagementSystem.PresentationLayer.Handlers;

public static class PersonHandler
{
    public static void DemonstrateActions(ISchoolMemberActions? person, object? user)
    {
        try
        {
            if (person == null)
            {
                throw new Exceptions.PersonNotFoundException("No person provided.");
            }

            var personHelper = new PersonHelper();
            personHelper.CheckAndDemonstrateActions(person, user);
        }
        catch (Exceptions.PersonNotFoundException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}