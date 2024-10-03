using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.Interfaces.Validation;

public interface IValidationHelper
{
    bool ValidateStudentList(List<Student?>? students);
    void ValidateUser(IUser? user);
    Student? SelectAndValidateStudent(List<Student?>? students);
    void CheckHasPermission(IUser? user, bool isAdmin = false, bool isTeacherOrAdmin = false);
    void ValidateNotNull<T>(List<T>? list, string listName);
    void ValidateAccess(IUser? user, Student? student);
    void ValidateUserPermission(IUser? user, bool isAdmin = false, bool isTeacherOrAdmin = false);
}