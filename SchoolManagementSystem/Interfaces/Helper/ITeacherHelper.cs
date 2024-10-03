using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.Interfaces.Helper;

public interface ITeacherHelper
{
    void DisplayMenuOptions(string[] options);
    int GetValidatedUserChoice(int maxOptions);
    void DisplayTeacherDetails(List<Teacher?>? teachers, object? user);
}