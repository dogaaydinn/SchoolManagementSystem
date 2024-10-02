using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;

namespace SchoolManagementSystem.PresentationLayer.Menu.AdminMenu;

public static class AdminStudentMenu
{
    public static void DisplayStudentMenu(List<Student?>? students, object? user)
    {
        while (true)
        {
            Console.WriteLine("\nAdmin Student Menu:");
            Console.WriteLine("1. Display Student Details");
            Console.WriteLine("2. Update Student ID");
            Console.WriteLine("3. Update Student Name");
            Console.WriteLine("4. Add New Student");
            Console.WriteLine("5. Remove Student");
            Console.WriteLine("6. Display Student Grades");
            Console.WriteLine("7. Update Student GPA");
            Console.WriteLine("8. Get Student By ID");
            Console.WriteLine("9. List Students in Courses");
            Console.WriteLine("10. Display Student Actions");
            Console.WriteLine("11. Enroll Students in Courses");
            Console.WriteLine("12. Remove Student Interactive");
            Console.WriteLine("13. Assign Courses To Students");
            Console.WriteLine("14. Record Grades For Students");
            Console.WriteLine("15. Exit");
            
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
                    var student = SchoolHandler.SelectStudent(students);
                    if (student != null)
                    {
                        StudentHandler.DisplayStudentDetails(student);
                    }
                    break;
                case "2":
                    if (students != null)
                    {
                        var nonNullableStudents = students.OfType<Student>().ToList();
                        StudentHandler.UpdateStudentId(nonNullableStudents, (IUser)user);
                    }
                    else
                    {
                        Console.WriteLine("Students list is null.");
                    }
                    break;
                case "3":
                    student = SchoolHandler.SelectStudent(students);
                    if (student != null)
                    {
                        StudentHandler.UpdateStudentName(student, (IUser)user);
                    }
                    break;
                case "4":
                    StudentHandler.AddNewStudent(students.OfType<Student>().ToList(), (IUser)user);
                    break;
                case "5":
                    student = SchoolHandler.SelectStudent(students);
                    if (student != null)
                    {
                        StudentHandler.RemoveStudent(students.OfType<Student>().ToList(), student, (IUser)user);
                    }
                    break;
                case "6":
                    student = SchoolHandler.SelectStudent(students);
                    if (student != null)
                    {
                        StudentHandler.DisplayStudentGrades(student, new List<Course>(), (IUser)user);
                    }
                    break;
                case "7":
                    student = SchoolHandler.SelectStudent(students);
                    if (student != null)
                    {
                        if (user != null)
                        {
                            StudentHandler.UpdateStudentGpa(student, (IUser)user);
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

                case "8":
                    if (students == null) return;
                    student = SchoolHandler.SelectStudent(students);
                    if (student != null)
                    {
                        var validStudents = students.Where(s => s != null).Cast<Student>().ToList();
                        StudentHandler.GetStudentById(validStudents);
                    }
                    else
                    {
                        Console.WriteLine("Error: Student is null.");
                    }
                    break;
                case "9":
                    if (user != null)
                    {
                        CourseHandler.ListStudentsInCourses(new List<Course>(), (IUser)user);
                    }
                    else
                    {
                        Console.WriteLine("Error: User is null.");
                    }
                    break;

                case "10":
                    student = SchoolHandler.SelectStudent(students);
                    if (student != null)
                    {
                        if (user != null)
                        {
                            StudentHandler.DisplayStudentActions(student, (IUser)user);
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

                case "11":
                    if (user != null)
                    {
                        CourseHandler.EnrollStudentsInCourses(new List<Course>(), new List<Student?>(), (IUser)user);
                    }
                    else
                    {
                        Console.WriteLine("Error: User is null.");
                    }
                    break;

                case "12":
                    if (user != null)
                    {
                        CourseHandler.RemoveStudentInteractive(new List<Course>(), (IUser)user);
                    }
                    else
                    {
                        Console.WriteLine("Error: User is null.");
                    }
                    break;

                case "13":
                    if (user != null)
                    {
                        SchoolHandler.AssignCoursesToStudents(new List<Course>(), new List<Student?>(), (IUser)user);
                    }
                    else
                    {
                        Console.WriteLine("Error: User is null.");
                    }
                    break;

                case "14":
                    if (user != null)
                    {
                        SchoolHandler.RecordGradesForStudents(new List<Course>(), (IUser)user);
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