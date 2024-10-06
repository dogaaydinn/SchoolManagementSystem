using System.Text;
using SchoolManagementSystem.Interfaces.User;

namespace SchoolManagementSystem.Models.Abstract;

public abstract class Person : IUser
{
    #region Constructors

    protected Person(string firstName, string lastName, DateTime dateOfBirth, bool ısAdmin, string password)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        IsAdmin = ısAdmin;
        Password = password;
    }

    #endregion
    #region Properties

    private string FirstName { get; }
    private string LastName { get; }
    private DateTime DateOfBirth { get; }
    protected internal string Password { get; private set; }
    public int Id { get; set; }
    public bool IsAdmin { get; }
    #endregion
    #region Methods

    public string GetPassword()
    {
        return Password;
    }
    
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
    
    public void SetPassword(string password)
    {
        Password = password;
    }

    public abstract void DisplayDetails();

    #endregion
}