using SchoolManagementSystem.Interfaces.User;

namespace SchoolManagementSystem.Models.Concrete;

public class Admin : IUser
{ 
    public int GetUserId()
    {
        return 0;
    }
    public string GetFullName()
    {
        return "Admin";
    }
}