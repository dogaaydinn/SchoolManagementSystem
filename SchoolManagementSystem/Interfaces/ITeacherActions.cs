
namespace SchoolManagementSystem.Interfaces;

public interface ITeacherActions : IPersonActions
{
    void Teach();
    void CheckAttendance();
    void GradePapers();
    void PrepareLesson();
    void AttendMeeting();
    
}