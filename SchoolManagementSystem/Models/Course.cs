using SchoolManagementSystem.Interfaces;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.Models;

public class Course : ISchoolActions
{
    #region Constructors

    public Course(int courseId, string courseName, Teacher teacher, int credits)
    {
        _courseId = courseId;
        _courseName = courseName;
        _assignedTeacher = teacher;
        _credits = credits;
        _enrolledStudents = new List<Student?>();
        _studentGrades = new Dictionary<int, Grade>();
    }

    #endregion
    #region Fields

    private const int MaxStudents = 30;

    private int _courseId;
    private string _courseName;
    private readonly List<Student?> _enrolledStudents;
    private readonly Dictionary<int, Grade> _studentGrades;
    private readonly Teacher _assignedTeacher;
    private int _credits;

    #endregion
    #region Methods

    public void SetCredits(int newCredits)
    {
        _credits = newCredits;
    }

    public int GetCourseId()
    {
        return _courseId;
    }

    public int GetCredits()
    {
        return _credits;
    }
    
    public void SetCourseId(int newCourseId)
    {
        _courseId = newCourseId;
    }
    public List<double> GetGrades()
    {
        return _studentGrades.Values.Select(grade => grade.GetGradeValue()).ToList();
    }

    public string GetAssignedTeacherName()
    {
        return $"Teacher: {_assignedTeacher.GetTeacherFullName()}";
    }
    
    public int GetAssignedTeacher()
    {
        return _assignedTeacher.GetTeacherId();
    }

    public void EnrollStudent(Student student)
    {
        if (_enrolledStudents.Count == MaxStudents)
        {
            Console.WriteLine($"Course {_courseName} is full. Cannot enroll more students.");
            return;
        }

        if (IsStudentEnrolled(student))
        {
            Console.WriteLine($"Student {student.GetStudentFullName()} is already enrolled in {GetCourseName()}.");
            return;
        }

        _enrolledStudents.Add(student);
        Console.WriteLine($"Enrolled {student.GetStudentFullName()} in {GetCourseName()}.");
    }


    public void UnenrollStudent(Student? student)
    {
        if (!IsStudentEnrolled(student))
        {
            Console.WriteLine($"Student {student.GetStudentFullName()} is not enrolled in {GetCourseName()}.");
            return;
        }

        _enrolledStudents.Remove(student);
        Console.WriteLine($"Unenrolled {student.GetStudentFullName()} from {GetCourseName()}.");
    }

    public bool IsStudentEnrolled(Student? student)
    {
        return _enrolledStudents.Contains(student);
    }

    public string GetCourseName()
    {
        return _courseName;
    }
    public void SetCourseName(string newCourseName)
    {
        _courseName = newCourseName;
    }
    public List<Student?> GetEnrolledStudents()
    {
        return _enrolledStudents;
    }

    public void ListStudents()
    {
        Console.WriteLine($"Course {_courseName} has the following students:");
        foreach (var student in _enrolledStudents)
        {
            Console.WriteLine(student!.GetStudentFullName());
        }
    }

    public void AssignGrade(Student? student, double grade)
    {
        if (!IsStudentEnrolled(student))
        {
            Console.WriteLine($"Student {student!.GetStudentFullName()} is not enrolled in {GetCourseName()}.");
            return;
        }

        var gradeObj = _studentGrades.GetValueOrDefault(student!.GetStudentId(), new Grade(student.GetStudentId(), _courseId, grade));
        gradeObj.UpdateValue(grade);
        _studentGrades[student.GetStudentId()] = gradeObj;

        Console.WriteLine($"Assigned/Updated grade for {student.GetStudentFullName()} in course {_courseName}.");

        student.CalculateGpa();
    }

    public double GetAssignedGrades(Student? student)
    {
        return _studentGrades.TryGetValue(student!.GetStudentId(), out var grade) ? grade.GetGradeValue() : -1.0;
    }

    public override string ToString()
    {
        return $"{_courseId}: {_courseName}, Teacher: {_assignedTeacher.GetTeacherFullName()}";
    }

    public void ListGrades()
    {
        if (_studentGrades.Count == 0)
        {
            Console.WriteLine($"No grades assigned yet for the course {_courseName}.");
            return;
        }

        Console.WriteLine($"Course {_courseName} has the following grades:");
        foreach (var gradeEntry in _studentGrades)
        {
            Console.WriteLine($"Student ID: {gradeEntry.Key}, Grade: {gradeEntry.Value.GetGradeValue()}");
        }
    }

    public void UpdateStudentGpa(Student? student, double newGpa)
    {
        if (!IsStudentEnrolled(student))
        {
            Console.WriteLine($"Student {student!.GetStudentFullName()} is not enrolled in {GetCourseName()}.");
            return;
        }

        try
        {
            student.SetGpa(newGpa);
            Console.WriteLine($"Updated GPA for {student.GetStudentFullName()} in course {_courseName}.");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    #endregion
    #region ISchoolActions

    public void AssignCourse(Course course)
    {
        Console.WriteLine($"Course {course.GetCourseName()} assigned.");
    }

    public void RemoveCourse(Course course)
    {
        Console.WriteLine($"Course {course.GetCourseName()} removed.");
    }

    #endregion
}