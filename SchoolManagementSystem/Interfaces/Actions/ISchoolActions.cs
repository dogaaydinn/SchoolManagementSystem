using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.Interfaces.Actions;

public interface ISchoolActions
{
    void AssignCourse(Course course);
    void RemoveCourse(Course course);
}