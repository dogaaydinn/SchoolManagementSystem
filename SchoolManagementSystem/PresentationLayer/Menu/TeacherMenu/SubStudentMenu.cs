using SchoolManagementSystem.Data;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer;
using SchoolManagementSystem.PresentationLayer.Handlers;

public static class SubStudentMenu
{
    public static void DisplayStudentMenu(List<Student>? students, object user)
    {
        var nullableStudents = students.Cast<Student?>().ToList();
        var userToValidate = user as IUser;
        var schoolHelper = new SchoolHelper(); // Create an instance of SchoolHelper

        while (true)
        {
            Console.WriteLine("\nStudent Menu:");
            DisplayMenuOptions();

            var choice = Console.ReadLine();

            if (string.IsNullOrEmpty(choice))
            {
                Console.WriteLine("Input cannot be empty. Please try again.");
                continue;
            }

            if (!HandleMenuChoice(choice, students, nullableStudents, userToValidate, schoolHelper))
            {
                return;
            }
        }
    }

    private static void DisplayMenuOptions()
    {
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
    }

    private static bool HandleMenuChoice(string choice, List<Student> students, List<Student?> nullableStudents, IUser userToValidate, SchoolHelper schoolHelper)
    {
        Student? selectedStudent = null;

        switch (choice)
        {
            case "1":
                selectedStudent = schoolHelper.SelectStudent(students); // Use the instance method
                if (selectedStudent != null)
                {
                    StudentHandler.DisplayStudentDetails(selectedStudent);
                }
                break;
            case "2":
                selectedStudent = schoolHelper.SelectStudent(students); // Use the instance method
                if (selectedStudent != null)
                {
                    StudentHelper.DisplayGrades(selectedStudent);
                }
                break;
            case "3":
                SchoolHandler.DemonstrateActions(students, userToValidate);
                break;
            case "4":
                selectedStudent = schoolHelper.SelectStudent(students); // Use the instance method
                if (selectedStudent != null)
                {
                    StudentHandler.UpdateStudentGpa(selectedStudent, userToValidate);
                }
                break;
            case "5":
                var studentById = schoolHelper.SelectStudent(students); // Use the instance method
                if (studentById != null)
                {
                    StudentHelper.GetStudentById(students);
                }
                break;
            case "7":
                SchoolHandler.RecordGradesForStudents(new List<Course>(), userToValidate);
                break;
            case "8":
                var courses = DataProvider.GetCourses(new List<Teacher?>(), new List<Student>());
                SchoolHandler.RecordGradesForStudents(courses, userToValidate);
                break;
            case "9":
                StudentHandler.DisplayAllStudents(nullableStudents);
                break;
            case "10":
                var teachers = DataProvider.GetTeachers(new List<Course>());
                TeacherHandler.DisplayAllTeachers(teachers);
                break;
            case "11":
                if (userToValidate != null)
                {
                    StudentHandler.AddNewStudent(students, userToValidate);
                }
                else
                {
                    Console.WriteLine("Error: User is null.");
                }
                break;
            case "12":
                selectedStudent = schoolHelper.SelectStudent(students); // Use the instance method
                if (selectedStudent != null)
                {
                    if (userToValidate != null)
                    {
                        StudentHandler.RemoveStudent(nullableStudents, selectedStudent, userToValidate);
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
            case "13":
                if (userToValidate != null)
                {
                    CourseHandler.ListStudentsInCourses(new List<Course>(), nullableStudents, userToValidate);
                }
                else
                {
                    Console.WriteLine("Error: User is null.");
                }
                break;
            case "14":
                return false;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }

        return true;
    }
}