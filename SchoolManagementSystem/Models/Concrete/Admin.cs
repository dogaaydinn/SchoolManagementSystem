using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models.Abstract;

namespace SchoolManagementSystem.Models.Concrete;

public class Admin : Person, IUser
{
    public Admin(string firstName, string lastName, DateTime dateOfBirth, int adminId)
        : base(firstName, lastName, dateOfBirth, true)
    {
        AdminId = adminId;
    }

    private int AdminId { get; set; }

    public new int Id
    {
        get => AdminId;
        set => AdminId = value;
    }

    public override void DisplayDetails()
    {
        Console.WriteLine($"Admin ID: {AdminId}, Name: {GetFullName()}, Date of Birth: {GetDateOfBirth():d}");
    }

    public new void DisplayUserInfo()
    {
        DisplayDetails();
    }
}