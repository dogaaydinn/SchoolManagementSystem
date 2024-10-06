using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models.Abstract;

namespace SchoolManagementSystem.Models.Concrete;

public class Admin : SchoolMember, IUser
{
    #region Fields
    private static readonly List<SchoolMember?> SchoolMembers = new();
    private static readonly List<Admin> Admins = new();
    private static int _currentAdminId = 21;
    private new string Password { get; set; }
    private string Username { get; }
    private int AdminId { get; set; }
    #endregion
    #region Constructors
    public Admin(string firstName, string lastName, DateTime dateOfBirth, int adminId, string password)
        : base(firstName, lastName, dateOfBirth, password)
    {
        AdminId = adminId;
        Password = password;
        Username = $"{firstName}{lastName}";
    }
    #endregion
    #region Methods
    public new int Id
    {
        get => AdminId;
        set => AdminId = value;
    }

    public new void DisplayUserInfo()
    {
        DisplayDetails();
    }

    private static int GenerateAdminId()
    {
        return _currentAdminId++;
    }

    public static void AddAdmin(string firstName, string lastName, DateTime dateOfBirth, string password)
    {
        var adminId = GenerateAdminId();
        var newAdmin = new Admin(firstName, lastName, dateOfBirth, adminId, password);
        Admins.Add(newAdmin);
        SchoolMembers.Add(newAdmin);
    }

    public override void DisplayDetails()
    {
        Console.WriteLine($"Admin ID: {AdminId}, Name: {GetFullName()}, Date of Birth: {GetDateOfBirth():d}, Username: {Username}");
    }

    public new void SetPassword(string password)
    {
        Password = password;
    }

    public new string GetPassword()
    {
        return Password;
    }
    #endregion
}