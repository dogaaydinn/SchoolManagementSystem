namespace SchoolManagementSystem.Models.Concrete;

public class Grade
{
    
    #region Properties

    private int StudentId { get; }
    private int CourseId { get; }
    private double Value { get; set; }

    #endregion
    #region Constructors

    public Grade(int studentId, int courseId, double value)
    {
        StudentId = studentId;
        CourseId = courseId;
        UpdateValue(value);
    }

    #endregion
    #region Methods
    
    public void UpdateValue(double newValue)
    {
        if (newValue is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(newValue), "Grade must be between 0 and 100.");

        Value = newValue;
    }
    public double GetGradeValue()
    {
        return Value;
    }

    public override string ToString()
    {
        return $"Student ID: {StudentId}, Course ID: {CourseId}, Grade: {Value}";
    }
    #endregion
    
}