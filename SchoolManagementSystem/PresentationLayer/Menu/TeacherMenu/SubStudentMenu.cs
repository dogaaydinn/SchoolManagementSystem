using SchoolManagementSystem.Data;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;

namespace SchoolManagementSystem.PresentationLayer.Menu.TeacherMenu;

public static class SubStudentMenu
{
    public static void DisplayStudentMenu(List<Student> students, object? user)
    {
        var nullableStudents = students.Cast<Student?>().ToList();

        while (true)
        {
            Console.WriteLine("\nStudent Menu:");
            Console.WriteLine("1. Display Student Details");
            Console.WriteLine("2. Display Student Grades");
            Console.WriteLine("3. Display Student Actions");
            Console.WriteLine("4. Update Student GPA");
            Console.WriteLine("5. Get Student By ID");
            Console.WriteLine("6. Demonstrate Student Methods");
            Console.WriteLine("7. Assign Grade to Student");
            Console.WriteLine("8. Assign Courses to Students");
            Console.WriteLine("9. Display Student Names");
            Console.WriteLine("10. Display Teacher Names");
            Console.WriteLine("11. Add New Student");
            Console.WriteLine("12. Remove Student");
            Console.WriteLine("13. List Students in Courses");
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
        var selectedStudent = SchoolHandler.SelectStudent(students);
        if (selectedStudent != null)
        {
            StudentHandler.DisplayStudentDetails(selectedStudent);
        }
        break;
    case "2":
        StudentHandler.DisplayStudentGrades(SchoolHandler.SelectStudent(students), new List<Course>(), user);
        break;
    case "3":
        // Implement Display Student Actions logic
        break;
    case "4":
        StudentHandler.UpdateStudentGpa(SchoolHandler.SelectStudent(students), user);
        break;
    case "5":
        var studentById = SchoolHandler.SelectStudent(students);
        if (studentById != null)
        {
            StudentHandler.GetStudentById(students);
        }
        break;
    case "6":
        SchoolHandler.DemonstrateStudentActions(SchoolHandler.SelectStudent(students), user);
        break;
    case "8":
        var courses = new List<Course>();
        SchoolHandler.RecordGradesForStudents(courses, user);
        break;
    case "10":
        StudentHandler.DisplayAllStudents(nullableStudents);
        break;
    case "11":
        var availableCourses = DataProvider.GetCourses(new List<Teacher?>(), new List<Student>());

        var teachers = DataProvider.GetTeachers(availableCourses);
        TeacherHandler.DisplayAllTeachers(teachers);
        break;
    case "12":
        if (user != null)
        {
            StudentHandler.AddNewStudent(students, (IUser)user);
        }
        else
        {
            Console.WriteLine("Error: User is null.");
        }
        break;
    case "13":
        var studentToRemove = SchoolHandler.SelectStudent(students);
        if (studentToRemove != null)
        {
            if (user != null)
            {
                StudentHandler.RemoveStudent(nullableStudents, studentToRemove, (IUser)user);
            }
            else
            {
                Console.WriteLine("Error: User is null.");
            }
        }
        else
        {
            Console.WriteLine("Error: Student is null.");
        }
        break;
    case "14":
        if (user != null)
        {
            CourseHandler.ListStudentsInCourses(new List<Course>(), (IUser)user);
        }
        else
        {
            Console.WriteLine("Error: User is null.");
        }
        break;
    case "15":
        return;
    default:
        Console.WriteLine("Invalid choice. Please try again.");
        break;
}
        }
    }
}