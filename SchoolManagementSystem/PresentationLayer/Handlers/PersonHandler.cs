using SchoolManagementSystem.BusinessLogicLayer.Exceptions;
using SchoolManagementSystem.Interfaces;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.PresentationLayer.Handlers;

public static class PersonHandler
{
    public static void DemonstrateActions(ISchoolMemberActions? person, object? user)
    {
        Exceptions.CheckPersonNotNull(person);
        Exceptions.CheckPermission(user);

        Console.WriteLine("Demonstrating common actions:");
        if (person == null) return;
        person.PerformDailyRoutine();
        person.Communicate();
        person.Rest();

        DemonstrateSpecificActions(person, user);
    }

    private static void DemonstrateSpecificActions(ISchoolMemberActions schoolMember, object? user)
    {
        switch (schoolMember)
        {
            case ITeacherActions teacher:
                if (HasPermission(user, isTeacherOrAdmin: true))
                {
                    DemonstrateTeacherActions(teacher);
                }
                else
                {
                    Console.WriteLine("You do not have permission to demonstrate teacher actions.");
                }
                break;
            case IStudentActions student:
                if (HasPermission(user))
                {
                    DemonstrateStudentActions(student);
                }
                else
                {
                    Console.WriteLine("You do not have permission to demonstrate student actions.");
                }
                break;
            default:
                Console.WriteLine($"Unknown person type: {schoolMember.GetType().Name}.");
                break;
        }
    }

    private static bool HasPermission(object? user, bool isTeacherOrAdmin = false)
    {
        if (user is Admin) return true;
        switch (isTeacherOrAdmin)
        {
            case true when user is Teacher:
            case false when user is Student:
                return true;
            default:
                return false;
        }
    }

    private static void DemonstrateTeacherActions(ITeacherActions teacher)
    {
        DemonstrateActions("teacher-specific", new []
        {
            teacher.Teach,
            teacher.CheckAttendance
        });
    }

    private static void DemonstrateStudentActions(IStudentActions student)
    {
        DemonstrateActions("student-specific", new[]
        {
            student.Learn,
            student.TakeTest,
            student.SubmitAssignment,
            student.Study,
            student.ParticipateInClass,
            student.AttendClass,
            student.DoHomework
        });
    }

    private static void DemonstrateActions(string actionType, Action[] actions)
    {
        Console.WriteLine($"Demonstrating {actionType} actions:");
        foreach (var action in actions)
        {
            action();
        }
    }
}