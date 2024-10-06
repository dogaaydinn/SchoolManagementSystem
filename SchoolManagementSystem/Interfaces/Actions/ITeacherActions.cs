namespace SchoolManagementSystem.Interfaces.Actions;

public interface ITeacherActions : ISchoolMemberActions
{
    void Teach();
    void CheckAttendance();
    void GradePapers();
    void PrepareLesson();
    void AttendMeeting();
}