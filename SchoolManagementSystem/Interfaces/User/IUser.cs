namespace SchoolManagementSystem.Interfaces.User
{
    public interface IUser
    {
        int Id { get; set; }
        void DisplayUserInfo();
        string GetFullName();
    }
}