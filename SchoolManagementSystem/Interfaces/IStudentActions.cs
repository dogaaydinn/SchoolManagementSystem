namespace SchoolManagementSystem.Interfaces;

public interface IStudentActions : ISchoolMemberActions
{
    void Learn();
    void TakeTest();
    void SubmitAssignment();
    void Study();
    void ParticipateInClass();
    void AttendClass();
    void DoHomework();
    string GetFullName();
}