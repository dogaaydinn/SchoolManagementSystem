using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Interfaces;

public interface ICourseManagement
{
    void AssignCourse(Course course);
    void RemoveCourse(Course course);
}