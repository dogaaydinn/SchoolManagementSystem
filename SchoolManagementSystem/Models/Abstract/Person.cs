using System.Text;
using SchoolManagementSystem.Interfaces.User;

namespace SchoolManagementSystem.Models.Abstract;

public abstract class Person : IUser
{
    #region Properties
    private string FirstName { get; set; }
    private string LastName { get; set; }
    private DateTime DateOfBirth { get; set; }
    private string Password { get; set; } 
    public int Id { get; set; }
    #endregion
    #region Constructors
    protected Person(string firstName, string lastName, DateTime dateOfBirth, bool ısAdmin)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        IsAdmin = ısAdmin;
        Password = GenerateRandomPassword();
    }
    #endregion
    #region Methods
    public bool IsAdmin { get; }
    public void SetPassword(string password)
    {
        Password = password;
    }

    public bool ValidatePassword(string password)
    {
        return Password == password;
    }
    public void GeneratePassword()
    {
        Password = Guid.NewGuid().ToString("N").Substring(0, 8); // Example password generation
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
    private string GenerateRandomPassword()
    {
        const int passwordLength = 8;
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        StringBuilder password = new StringBuilder();
        Random random = new Random();

        for (int i = 0; i < passwordLength; i++)
        {
            password.Append(validChars[random.Next(validChars.Length)]);
        }

        return password.ToString();
    }

    public string GetPassword()
    {
        return Password;
    }
    
    public int GetId()
    {
        return Id;
    }
    public abstract void DisplayDetails();
    #endregion
}