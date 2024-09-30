using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;

namespace SchoolManagementSystem.PresentationLayer.Menu;

public static class CourseMenu
{
    public static void DisplayCourseMenu(List<Course>? courses, List<Student> students)
    {
        while (true)
        {
            Console.WriteLine("\nCourse Operations:");
            Console.WriteLine("1. Display Course Details");
            Console.WriteLine("2. List Students in Courses");
            Console.WriteLine("3. Assign Grade to Student");
            Console.WriteLine("4. Display Total Courses");
            Console.WriteLine("5. Display Students");
            Console.WriteLine("6. Display Course Grades");
            Console.WriteLine("7. Enroll Students in Courses");
            Console.WriteLine("8. Remove Student Interactive");
            Console.WriteLine("9. Check Student Enrollment");
            Console.WriteLine("10. Display Student Grade");
            Console.WriteLine("11. Update Course Credits");
            Console.WriteLine("12. Back to Main Menu");
            Console.Write("Enter your choice (1-12): ");
            var choice = Console.ReadLine();

            if (!int.TryParse(choice, out int selectedOption) || selectedOption < 1 || selectedOption > 12)
            {
                Console.WriteLine("Invalid choice. Please enter a number between 1 and 12.");
                continue;
            }

            switch (selectedOption)
            {
                case 1:
                    CourseHandler.DisplayCourseDetails(courses);
                    break;
                case 2:
                    CourseHandler.ListStudentsInCourses(courses);
                    break;
                case 3:
                    StudentHandler.AssignGradeToStudent(courses, students);
                    break;
                case 4:
                    CourseHandler.DisplayTotalCourses(courses);
                    break;
                case 5:
                    CourseHandler.DisplayStudents(courses);
                    break;
                case 6:
                    CourseHandler.DisplayCourseGrades(courses);
                    break;
                case 7:
                    CourseHandler.EnrollStudentsInCourses(courses, students);
                    break;
                case 8:
                    CourseHandler.RemoveStudentInteractive(students, courses);
                    break;
                case 9:
                    CourseHandler.CheckStudentEnrollment(courses, students);
                    break;
                case 10:
                    CourseHandler.DisplayStudentGrade(courses, students);
                    break;
                case 11:
                    CourseHandler.UpdateCourseCredits(courses);
                    break;
                case 12:
                    return; // Return to Main Menu
            }
        }
    }
}
