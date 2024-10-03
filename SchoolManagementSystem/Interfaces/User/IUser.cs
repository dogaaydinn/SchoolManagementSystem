namespace SchoolManagementSystem.Interfaces.User;

public interface IUser
{
    int Id { get; set; }
    bool IsAdmin { get; }
    void DisplayUserInfo();
    string GetFullName();
}