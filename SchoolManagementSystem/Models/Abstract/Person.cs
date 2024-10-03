using SchoolManagementSystem.Interfaces.User;

namespace SchoolManagementSystem.Models.Abstract;

public abstract class Person : IUser
{
    #region Properties
    private string FirstName { get; set; }
    private string LastName { get; set; }
    private DateTime DateOfBirth { get; set; }
    public int Id { get; set; }
    #endregion
    #region Constructors
    protected Person(string firstName, string lastName, DateTime dateOfBirth, bool ısAdmin)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        IsAdmin = ısAdmin;
    }
    #endregion
    #region Methods
    public bool IsAdmin { get; }

    public void DisplayUserInfo()
    {
        Console.WriteLine($"ID: {Id}");
        Console.WriteLine($"Name: {GetFullName()}");
        Console.WriteLine($"Date of Birth: {DateOfBirth:dd/MM/yyyy}");
        Console.WriteLine($"Age: {GetAge()}");
    }

    protected DateTime GetDateOfBirth()
    {
        return DateOfBirth;
    }

    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }

    private int GetAge()
    {
        var today = DateTime.Today;
        var age = today.Year - GetDateOfBirth().Year;
        if (GetDateOfBirth().Date > today.AddYears(-age)) age--;
        return age;
    }

    public abstract void DisplayDetails();
    #endregion
}