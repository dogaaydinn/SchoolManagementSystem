using SchoolManagementSystem.Interfaces;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models.Abstract;

namespace SchoolManagementSystem.Models.Concrete;

public class Teacher : SchoolMember, ITeacherActions
{
    #region Constructors

    public Teacher(string firstName, string lastName, DateTime dateOfBirth, int teacherId, string subject)
        : base(firstName, lastName, dateOfBirth)
    {
        TeacherId = teacherId;
        Subject = subject;
    }

    #endregion
    #region Properties
    
    private int TeacherId { get; set; }
    private string Subject { get; set; }

    #endregion
    #region IPersonActions
    
    public void Teach()
    {
        Console.WriteLine($"{GetFullName()} is teaching {GetSubject()}.");
    }

    public void CheckAttendance()
    {
        Console.WriteLine($"{GetFullName()} is checking attendance for {GetSubject()} lesson.");
    }
    
    public void GradePapers()
    {
        Console.WriteLine($"{GetFullName()} is preparing to grade papers for {GetSubject()} lesson");
    }
    public void PrepareLesson()
    {
        Console.WriteLine($"{GetFullName()} is preparing lesson materials for {GetSubject()} lesson");
    }

    public void AttendMeeting()
    {
        Console.WriteLine($"{GetFullName()} is attending the meeting {GetSubject()} lesson.");
    }
    
    #endregion
    #region Methods

    public int GetTeacherId()
    {
        return TeacherId;
    }

    public string GetSubject()
    {
        return Subject;
    }

    public string GetTeacherFullName()
    {
        return GetFullName();
    }

    public void SetTeacherId(int teacherId)
    {
        TeacherId = teacherId;
    }

    public void SetSubject(string subject)
    {
        Subject = subject;
    }


    public override string ToString()
    {
        return $"{TeacherId}: {GetFullName()}, Subject: {Subject}";
    }

    public override void DisplayDetails()
    {
        Console.WriteLine(
            $"{GetFullName()} (Age: {GetAge()}), Subject: {GetSubject()}, Teacher ID: {GetTeacherId()}, (Date of Birth: {GetDateOfBirth():d})");
    }
    #endregion
}