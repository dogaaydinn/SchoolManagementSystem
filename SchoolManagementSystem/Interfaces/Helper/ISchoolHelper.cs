using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.Interfaces.Helper;

public interface ISchoolHelper
{
    Course? GetCourseFromUserInput(List<Course> courses);
    double? GetValidGrade(Student student);
    Student? SelectStudent(List<Student?> students);
    Course? SelectCourse(List<Course> courses);
    void DisplayStudents(List<Student?> students);
    void DisplayCourses(List<Course> courses);
}