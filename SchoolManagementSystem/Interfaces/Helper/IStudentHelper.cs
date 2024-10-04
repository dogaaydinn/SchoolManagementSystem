using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.Interfaces.Helper;

public interface IStudentHelper
{
    void DisplayStudentInfo(Student student);
    Student? GetStudentById(List<Student> students);
    void UpdateStudentId(Student student);
    void UpdateStudentGpa(Student student);
    void AddNewStudent(List<Student> students);
    void RemoveStudent(List<Student?> students, IUser user);
}