using SchoolManagementSystem.Interfaces;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models.Abstract;

namespace SchoolManagementSystem.Models.Concrete;

public class Student : SchoolMember, IStudentActions, IUser
{
    #region Constructors

    public Student(string firstName, string lastName, DateTime dateOfBirth, int studentId, double gpa, string studentName)
        : base(firstName, lastName, dateOfBirth)
    {
        StudentId = studentId;
        StudentName = studentName;
        Gpa = gpa;
        EnrolledCourses = new List<Course>();
        _courseGrades = new Dictionary<Course, int>();
    }

    #endregion
    #region Properties

    private int StudentId { get; set; }
    private string StudentName { get; set; }
    private double Gpa { get; set; }
    private List<Course> EnrolledCourses { get; }

    private readonly Dictionary<Course, int> _courseGrades;

    #endregion
    #region IPersonActions Members

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
    #region IStudentActions Members

    public void AttendClass()
    {
        Console.WriteLine($"{GetStudentFullName()} is attending class.");
    }

    public void DoHomework()
    {
        Console.WriteLine($"{GetStudentFullName()} is doing homework.");
    }

    public void Learn()
    {
        Console.WriteLine($"{GetFullName()} is learning.");
    }

    public void TakeTest()
    {
        Console.WriteLine($"{GetFullName()} is taking a test.");
    }

    public void SubmitAssignment()
    {
        Console.WriteLine($"{GetStudentFullName()} is submitting an assignment.");
    }

    public void Study()
    {
        Console.WriteLine($"{GetStudentFullName()} is studying.");
    }

    public void ParticipateInClass()
    {
        Console.WriteLine($"{GetStudentFullName()} is participating in class.");
    }

    #endregion
    #region Methods

    public int GetAssignedGrades(Course course)
    {
        return _courseGrades.GetValueOrDefault(course, -1);
    }

    public IReadOnlyList<Course> GetEnrolledCourses()
    {
        return EnrolledCourses.AsReadOnly();
    }

    public bool MeetsEnrollmentCriteria(Course course)
    {
        return !course.IsStudentEnrolled(this);
    }

    public string GetStudentFullName()
    {
        return GetFullName();
    }

    public void SetStudentName(string newName)
    {
        StudentName = newName;
    }

    public override void DisplayDetails()
    {
        Console.WriteLine($"{GetFullName()} (Age: {GetAge()}), Student ID: {GetStudentId()}, " +
                          $"(Date of Birth: {GetDateOfBirth():d}), GPA: {GetGpa()}, Enrolled Courses: {EnrolledCourses.Count}");
    }

    public override string ToString()
    {
        return $"{StudentId}: {GetFullName()}, GPA: {Gpa:F2}, Age: {GetAge()}, Assigned Courses: {EnrolledCourses.Count}, Date of Birth: {GetDateOfBirth():d}";
    }

    public void CalculateGpa()
    {
        if (_courseGrades.Count == 0)
        {
            Console.WriteLine("No grades available to calculate GPA.");
            return;
        }

        var totalGradePoints = _courseGrades.Sum(pair => pair.Value * pair.Key.GetCredits());
        var totalCredits = _courseGrades.Sum(pair => pair.Key.GetCredits());

        if (totalCredits == 0)
        {
            Console.WriteLine("Total credits cannot be zero.");
            return;
        }

        Gpa = totalGradePoints / totalCredits;
        Console.WriteLine($"GPA calculated: {Gpa:F2}");
    }

    public double GetGpa()
    {
        return Gpa;
    }

    public int GetStudentId()
    {
        return StudentId;
    }

    public void SetStudentId(int studentId)
    {
        StudentId = studentId;
    }

    public void UpdateStudentId(int newStudentId)
    {
        StudentId = newStudentId;
        Console.WriteLine($"Student ID updated to {newStudentId}.");
    }

    public void SetGpa(double gpa)
    {
        Gpa = gpa;
    }

    public void EnrollInCourse(Course course)
    {
        if (!EnrolledCourses.Contains(course))
        {
            EnrolledCourses.Add(course);
            course.EnrollStudent(this);
            Console.WriteLine($"{GetStudentFullName()} has been enrolled in {course.GetCourseName()}.");
        }
        else
        {
            Console.WriteLine($"{GetStudentFullName()} is already enrolled in {course.GetCourseName()}.");
        }
    }

    public void UnenrollFromCourse(Course course)
    {
        if (EnrolledCourses.Contains(course))
        {
            EnrolledCourses.Remove(course);
            course.UnenrollStudent(this);
            Console.WriteLine($"{GetStudentFullName()} has been unenrolled from {course.GetCourseName()}.");
        }
        else
        {
            Console.WriteLine($"{GetStudentFullName()} is not enrolled in {course.GetCourseName()}.");
        }
    }

    public bool IsEnrolledInCourse(int courseId)
    {
        return EnrolledCourses.Any(c => c.GetCourseId() == courseId);
    }

    public new void DisplayUserInfo()
    {
        DisplayDetails();
    }
    #endregion

    public int Id { get; set; }

}