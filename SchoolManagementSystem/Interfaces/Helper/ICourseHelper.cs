using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.Interfaces.Helper;

public interface ICourseHelper
{
    Course? GetCourseById(IEnumerable<Course?> courses);
    Course? GetCourseByName(IEnumerable<Course?> courses);
}