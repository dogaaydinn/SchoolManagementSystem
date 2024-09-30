namespace SchoolManagementSystem.Interfaces;

public interface IStudentActions : IPersonActions
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