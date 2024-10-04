using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.Interfaces.Actions;

public interface ICourseActions
{
    void StartCourse(Course course);
    void EndCourse(Course course);
}