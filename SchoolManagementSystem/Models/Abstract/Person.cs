namespace SchoolManagementSystem.Models.Abstract;

public abstract class Person
{
    #region Properties

    private string FirstName { get; set; }
    private string LastName { get; set; }
    private DateTime DateOfBirth { get; set; }

    #endregion
    #region Constructors

    protected Person(string firstName, string lastName, DateTime dateOfBirth)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
    }

    #endregion
    #region Methods

    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }

    protected DateTime GetDateOfBirth()
    {
        return DateOfBirth;
    }

    public abstract void DisplayDetails();

    #endregion
}