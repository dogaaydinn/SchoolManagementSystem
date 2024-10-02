using SchoolManagementSystem.Interfaces;

namespace SchoolManagementSystem.PresentationLayer.Handlers.ActionHandler;

public static class PersonActionDemonstrator
{
    public static void DemonstrateActions(ISchoolMemberActions schoolMember)
    {
        switch (schoolMember)
        {
            case ITeacherActions teacher:
                DemonstrateTeacherActions(teacher);
                break;
            case IStudentActions student:
                DemonstrateStudentActions(student);
                break;
            default:
                Console.WriteLine("Unknown school member actions.");
                break;
        }
    }

    private static void DemonstrateTeacherActions(ITeacherActions teacher)
    {
        try
        {
            teacher.Teach();
            teacher.CheckAttendance();
            teacher.GradePapers();
            teacher.PrepareLesson();
            teacher.AttendMeeting();
            Console.WriteLine($"{teacher.GetFullName()} has completed teaching and attendance check.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while demonstrating teacher actions: {ex.Message}");
        }
    }

    private static void DemonstrateStudentActions(IStudentActions student)
    {
        try
        {
            student.Learn();
            student.TakeTest();
            student.SubmitAssignment();
            student.Study();
            student.ParticipateInClass();
            Console.WriteLine($"{student.GetFullName()} has completed learning and participation in class.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while demonstrating student actions: {ex.Message}");
        }
    }
}