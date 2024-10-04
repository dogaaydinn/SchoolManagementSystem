using SchoolManagementSystem.Interfaces.Helper;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.PresentationLayer.Helpers;

public class SchoolHelper : ISchoolHelper
{
    public Course? GetCourseFromUserInput(List<Course> courses)
    {
        while (true)
        {
            Console.WriteLine("Enter the course ID to assign (or type 'done' to finish):");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input) || input.Equals("done", StringComparison.OrdinalIgnoreCase))
                return null;

            if (int.TryParse(input, out var courseId))
            {
                var course = courses?.Find(c => c.GetCourseId() == courseId);
                if (course != null) return course;

                Console.WriteLine("Course not found. Please try again.");
            }
            else
            {
                Console.WriteLine("Invalid course ID. Please try again.");
            }
        }
    }

    public double? GetValidGrade(Student student)
    {
        while (true)
        {
            Console.WriteLine($"Enter the grade for {student.GetStudentFullName()} (ID: {student.GetStudentId()}):");
            var input = Console.ReadLine();

            if (double.TryParse(input, out var grade) && grade is >= 0 and <= 100) return grade;

            Console.WriteLine("Invalid grade. Please enter a numeric value between 0 and 100.");
        }
    }

    public void DisplayStudents(List<Student?> students)
    {
        if (students == null || !students.Any())
        {
            Console.WriteLine("No students available to display.");
            return;
        }

        for (var i = 0; i < students.Count; i++)
        {
            var student = students[i];
            if (student != null)
                Console.WriteLine($"{i + 1}. {student.GetStudentFullName()} (ID: {student.GetStudentId()})");
        }
    }

    public void DisplayCourses(List<Course> courses)
    {
        if (courses == null || !courses.Any())
        {
            Console.WriteLine("No courses available to display.");
            return;
        }

        for (var i = 0; i < courses.Count; i++)
            Console.WriteLine($"{i + 1}. {courses[i].GetCourseName()} (ID: {courses[i].GetCourseId()})");
    }

    public Student? SelectStudent(List<Student?> students)
    {
        DisplayStudents(students);
        return GetUserSelection(students);
    }

    public Course? SelectCourse(List<Course> courses)
    {
        DisplayCourses(courses);
        return GetUserSelection(courses);
    }

    public Teacher? SelectTeacher(List<Teacher?> teachers)
    {
        DisplayTeachers(teachers);
        return GetUserSelection(teachers);
    }

    private void DisplayTeachers(List<Teacher?> teachers)
    {
        if (teachers == null || !teachers.Any())
        {
            Console.WriteLine("No teachers available to display.");
            return;
        }

        for (var i = 0; i < teachers.Count; i++)
        {
            var teacher = teachers[i];
            if (teacher != null)
                Console.WriteLine($"{i + 1}. {teacher.GetTeacherFullName()} (ID: {teacher.GetTeacherId()})");
        }
    }

    private T? GetUserSelection<T>(List<T?> items) where T : class
    {
        Console.WriteLine("Select an item by entering the corresponding number:");
        if (int.TryParse(Console.ReadLine(), out var index) && index >= 1 && index <= items.Count)
            return items[index - 1];
        Console.WriteLine("Invalid selection. Please try again.");
        return null;
    }
}