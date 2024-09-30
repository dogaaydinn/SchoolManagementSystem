using SchoolManagementSystem.Interfaces.User;

namespace SchoolManagementSystem.Models.Concrete
{
    public class Admin : IUser
    {
        private readonly int _ıd;
        private readonly string _name;
        

        public Admin(int adminId, string fullName)
        {
            _ıd = adminId;
            _name = fullName;
        }

        public int GetUserId() => _ıd;
        public string GetFullName() => _name;
        public string Id { get; }
        public string Name { get; }
    }
}