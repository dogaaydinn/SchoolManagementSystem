using SchoolManagementSystem.Interfaces;
using SchoolManagementSystem.Interfaces.Actions;

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

   public static void DemonstrateTeacherActions(ITeacherActions teacher)
    {
        ActionDemonstrator.DemonstrateActions("teacher-specific", new []
        {
            teacher.Teach,
            teacher.CheckAttendance,
            teacher.GradePapers,
            teacher.PrepareLesson,
            teacher.AttendMeeting
        });
    }

    public static void DemonstrateStudentActions(IStudentActions student)
    {
        ActionDemonstrator.DemonstrateActions("student-specific", new[]
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
}