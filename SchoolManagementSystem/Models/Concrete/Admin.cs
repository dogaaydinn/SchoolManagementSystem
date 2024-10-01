using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models.Abstract;

namespace SchoolManagementSystem.Models.Concrete;

public class Admin : Person, IUser
{
    public Admin(string firstName, string lastName, DateTime dateOfBirth, int adminId)
        : base(firstName, lastName, dateOfBirth)
    {
        AdminId = adminId;
    }

    public int AdminId { get; private set; }

    public int Id
    {
        get => AdminId;
        set => AdminId = value;
    }

    public override void DisplayDetails()
    {
        Console.WriteLine($"Admin ID: {AdminId}, Name: {GetFullName()}, Date of Birth: {GetDateOfBirth():d}");
    }

    public void DisplayUserInfo()
    {
        DisplayDetails();
    }
}