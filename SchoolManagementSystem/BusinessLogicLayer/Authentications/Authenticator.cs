using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Abstract;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.BusinessLogicLayer.Authentications;

public static class Authenticator
{
        
    public static SchoolMember? Authenticate(string firstName, string lastName, string password, int id)
    {
        var user = DataProvider.GetSchoolMemberByName(firstName, lastName);
        if (user != null && user.ValidatePassword(password) && user.Id == id)
        {
            return user;
        }

        return null;
    }
    public static SchoolMember? Register(string firstName, string lastName, DateTime dateOfBirth)
    {
        var studentId = DataProvider.GenerateStudentId();
        var newSchoolMember = new Student(firstName, lastName, dateOfBirth, studentId, 0);
        DataProvider.AddSchoolMember(newSchoolMember);

        Console.WriteLine($"User created successfully. Your ID is: {studentId}. Your password is: {newSchoolMember.GetPassword()}");
        Console.WriteLine("Warning: This information is important. Please note it down.");
        return newSchoolMember;
    }

    public static void ChangePassword(SchoolMember user, string newPassword)
    {
        user.SetPassword(newPassword);
        Console.WriteLine("Password changed successfully.");
    }
}