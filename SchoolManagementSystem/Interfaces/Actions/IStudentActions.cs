namespace SchoolManagementSystem.Interfaces.Actions;

public interface IStudentActions : ISchoolMemberActions
{
    void Learn();
    void TakeTest();
    void SubmitAssignment();
    void Study();
    void ParticipateInClass();
    void AttendClass();
    void DoHomework();
}