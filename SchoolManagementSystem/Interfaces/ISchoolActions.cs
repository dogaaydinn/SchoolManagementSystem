using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Interfaces;

public interface ISchoolActions 
{
    void AssignCourse(Course course);
    void RemoveCourse(Course course);
}