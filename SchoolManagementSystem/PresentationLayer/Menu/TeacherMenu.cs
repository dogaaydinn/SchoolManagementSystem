using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;

namespace SchoolManagementSystem.PresentationLayer.Menu;

public static class TeacherMenu
{
    public static void DisplayTeacherMenu(List<Teacher> teachers, List<Student> students, object user)
    {
        while (true)
        {
            Console.WriteLine("\nTeacher Menu:");
            Console.WriteLine("1. Demonstrate Actions");
            Console.WriteLine("2. Enroll Student in Course");
            Console.WriteLine("3. Assign Courses to Students");
            Console.WriteLine("4. Record Grades for Students");
            Console.WriteLine("5. Display All Details");
            Console.WriteLine("6. Assign Grade to Student");
            Console.WriteLine("7. Update Student GPA");
            Console.WriteLine("8. Display Student Names");
            Console.WriteLine("9. Display Course Names");
            Console.WriteLine("10. Display Teacher Names");
            Console.WriteLine("11. Update Teacher Details");
            Console.WriteLine("12. List Students in Courses");
            Console.WriteLine("13. Demonstrate Teacher Actions");
            Console.WriteLine("14. Exit");
            Console.Write("Enter your choice: ");

            var choice = Console.ReadLine();

            if (string.IsNullOrEmpty(choice))
            {
                Console.WriteLine("Input cannot be empty. Please try again.");
                continue;
            }

            switch (choice)
            {
                case "1":
                    SchoolHandler.DemonstrateActions(teachers, user);
                    break;
                case "2":
                    List<Course> courses = SchoolHandler.SelectCourse();
                    if (user is IUser iUser2)
                    {
                        var nullableStudents = students.Cast<Student?>().ToList();
                        SchoolHandler.EnrollStudentInCourse(nullableStudents, courses, iUser2);
                    }
                    else
                    {
                        Console.WriteLine("Invalid user type. Operation not permitted.");
                    }
                    break;
                case "3":
                    if (user is IUser iUser3)
                    {
                        SchoolHandler.AssignCoursesToStudents(teachers, students, iUser3);
                    }
                    else
                    {
                        Console.WriteLine("Invalid user type. Operation not permitted.");
                    }
                    break;
                case "4":
                    SchoolHandler.RecordGradesForStudents(course, students);
                    break;
                case "5":
                    SchoolHandler.DisplayAllDetails(courses, students, teachers, user);
                    break;
                case "6":
                    StudentHandler.AssignGradeToStudent(courses, students, user);
                    break;
                case "7":
                    StudentHandler.UpdateStudentGPA(students, user);
                    break;
                case "8":
                    TeacherHandler.DisplayStudentNames(students);
                    break;
                case "9":
                    TeacherHandler.DisplayCourseNames();
                    break;
                case "10":
                    TeacherHandler.DisplayTeacherNames(teachers);
                    break;
                case "11":
                    TeacherHandler.UpdateTeacherDetails(teachers, user);
                    break;
                case "12":
                    List<Course>? courses = SchoolHandler.SelectCourse();
                    if (courses == null)
                    {
                        Console.WriteLine("Courses list is null. Cannot list students in courses.");
                    }
                    else
                    {
                        CourseHandler.ListStudentsInCourses(courses, students);
                    }
                    break;
                case "13":
                    if (user is Teacher teacher)
                    {
                        TeacherHandler.DemonstrateTeacherMethods(teacher, teachers);
                    }
                    else
                    {
                        Console.WriteLine("Invalid user type. Operation not permitted.");
                    }
                    break;
                case "14":
                    return;
                case "15":
                    StudentHandler.UpdateStudentId(students, user);
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}