using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.Interfaces;
using SchoolManagementSystem.Interfaces.Helper;

namespace SchoolManagementSystem.PresentationLayer;

public class CourseHelper : ICourseHelper
{
    public Course? GetCourseById(IEnumerable<Course> courses)
    {
        var courseId = InputHelper.GetValidatedIntInput("Enter the Course ID:");
        return FindCourse(courses, c => c.GetCourseId() == courseId);
    }

    public Course? GetCourseByName(IEnumerable<Course> courses)
    {
        var courseName = InputHelper.GetValidatedStringInput("Enter the Course Name:");
        return FindCourse(courses, c => c.GetCourseName().Equals(courseName, StringComparison.OrdinalIgnoreCase));
    }

    private static Course? FindCourse(IEnumerable<Course> courses, Func<Course, bool> predicate)
    {
        return courses.FirstOrDefault(predicate);
    }
}