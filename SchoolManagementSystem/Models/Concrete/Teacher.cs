using SchoolManagementSystem.Interfaces;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models.Abstract;

namespace SchoolManagementSystem.Models.Concrete;

public class Teacher : SchoolMember, ITeacherActions, IUser
{
    #region Constructors

    public Teacher(string firstName, string lastName, DateTime dateOfBirth, int teacherId, string subject)
        : base(firstName, lastName, dateOfBirth)
    {
        TeacherId = teacherId;
        Subject = subject;
        FullName = $"{firstName} {lastName}";
    }

    #endregion
    #region Properties

    private int TeacherId { get; set; }
    private string Subject { get; set; }

    private string FullName { get; set; }

    #endregion
    #region ITeacherActions

    public void Teach()
    {
        Console.WriteLine($"{GetFullName()} is teaching {Subject}.");
    }

    public void CheckAttendance()
    {
        Console.WriteLine($"{GetFullName()} is checking attendance for {Subject} lesson.");
    }

    public void GradePapers()
    {
        Console.WriteLine($"{GetFullName()} is preparing to grade papers for {Subject} lesson");
    }

    public void PrepareLesson()
    {
        Console.WriteLine($"{GetFullName()} is preparing lesson materials for {Subject} lesson");
    }

    public void AttendMeeting()
    {
        Console.WriteLine($"{GetFullName()} is attending the meeting {Subject} lesson.");
    }

    #endregion
    #region IPersonActions

    public void Communicate()
    {
        Console.WriteLine($"{GetFullName()} is communicating.");
    }

    public void PerformDailyRoutine()
    {
        Console.WriteLine($"{GetFullName()} is performing daily routine.");
    }

    public void Rest()
    {
        Console.WriteLine($"{GetFullName()} is resting.");
    }

    #endregion
    #region Methods

    public override string ToString()
    {
        return $"{TeacherId}: {GetFullName()}, Subject: {Subject}";
    }

    public override void DisplayDetails()
    {
        Console.WriteLine(
            $"{GetFullName()} (Age: {GetAge()}), Subject: {Subject}, Teacher ID: {TeacherId}, (Date of Birth: {GetDateOfBirth():d})");
    }

    public new void DisplayUserInfo()
    {
        DisplayDetails();
    }
    public void SetTeacherFullName(string fullName)
    {
        FullName = fullName;
    }
    public string GetTeacherFullName()
    {
        return GetFullName();
    }

    public int GetTeacherId()
    {
        return TeacherId;
    }
    public string GetSubject()
    {
        return Subject;
    }

    public void SetSubject(string newSubject)
    {
        Subject = newSubject;
    }
    public void SetTeacherId(int newTeacherId)
    {
        TeacherId = newTeacherId;
    }
    #endregion

    public new int Id { get; set; }

}