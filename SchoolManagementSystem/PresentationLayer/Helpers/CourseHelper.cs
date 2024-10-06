using SchoolManagementSystem.Interfaces.Helper;
using SchoolManagementSystem.Models.Concrete;
using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.BusinessLogicLayer.Validations;

namespace SchoolManagementSystem.PresentationLayer.Helpers;

public class CourseHelper : ICourseHelper
{
    public static Course? GetCourseById(IEnumerable<Course?> courses)
    {
        try
        {
            var courseList = courses.ToList();
            ValidationHelper.ValidateList(courseList, "Courses list cannot be null or empty.");
            var courseId = InputHelper.GetValidatedIntInput("Enter the Course ID:");
            return FindCourse(courseList, c => c.GetCourseId() == courseId);
        }
        catch (ValidationException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while getting the course by ID: {ex.Message}");
            return null;
        }
    }

    public static Course? GetCourseByName(IEnumerable<Course?> courses)
    {
        try
        {
            var courseList = courses.ToList();
            ValidationHelper.ValidateList(courseList, "Courses list cannot be null or empty.");
            var courseName = InputHelper.GetValidatedStringInput("Enter the Course Name:");
            return FindCourse(courseList, c => c.GetCourseName().Equals(courseName, StringComparison.OrdinalIgnoreCase));
        }
        catch (ValidationException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while getting the course by name: {ex.Message}");
            return null;
        }
    }

    private static Course? FindCourse(IEnumerable<Course?> courses, Func<Course?, bool> predicate)
    {
        return courses.FirstOrDefault(predicate);
    }
}