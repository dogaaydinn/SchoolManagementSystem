
namespace SchoolManagementSystem.Interfaces;

public interface ITeacherActions : ISchoolMemberActions
{
    void Teach();
    void CheckAttendance();
    void GradePapers();
    void PrepareLesson();
    void AttendMeeting();
    string GetFullName();
    
}