using SchoolManagementSystem.BusinessLogicLayer.Validations;
using SchoolManagementSystem.Interfaces.User;
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
                    var student = ValidationHelper.SelectAndValidateStudent(students);
                    if (student != null) StudentHandler.DisplayStudentDetails(student);
                    break;
                case "2":
                    if (ValidationHelper.ValidateStudentList(students) && ValidationHelper.ValidateUser(user))
                        StudentHandler.UpdateStudentId(students.OfType<Student>().ToList(), (IUser)user);
                    break;
                case "3":
                    student = ValidationHelper.SelectAndValidateStudent(students);
                    if (student != null && ValidationHelper.ValidateUser(user))
                        StudentHandler.UpdateStudentName(new List<Student?> { student }, (IUser)user);
                    break;
                case "4":
                    if (ValidationHelper.ValidateStudentList(students) && ValidationHelper.ValidateUser(user))
                        StudentHandler.AddNewStudent(students.OfType<Student>().ToList(), (IUser)user);
                    break;
                case "5":
                    student = ValidationHelper.SelectAndValidateStudent(students);
                    if (student != null && ValidationHelper.ValidateUser(user))
                        StudentHandler.RemoveStudent(students.OfType<Student>().ToList(), student, (IUser)user);
                    break;
                case "6":
                    student = ValidationHelper.SelectAndValidateStudent(students);
                    if (student != null && ValidationHelper.ValidateUser(user))
                        StudentHelper.DisplayGrades(student);
                    break;
                case "7":
                    student = ValidationHelper.SelectAndValidateStudent(students);
                    if (student != null && ValidationHelper.ValidateUser(user))
                        StudentHandler.UpdateStudentGpa(student, (IUser)user);
                    break;
                case "8":
                    student = ValidationHelper.SelectAndValidateStudent(students);
                    if (student != null)
                        StudentHelper.GetStudentById(students.OfType<Student>().ToList());
                    break;
                case "9":
                    if (ValidationHelper.ValidateUser(user))
                        CourseHandler.ListStudentsInCourses(new List<Course>(), (IUser)user);
                    break;
                case "10":
                    student = ValidationHelper.SelectAndValidateStudent(students);
                    if (student != null && ValidationHelper.ValidateUser(user))
                        SchoolHandler.DemonstrateActions(student, user);
                    break;
                case "11":
                    if (ValidationHelper.ValidateUser(user))
                    {
                        var studentsToEnroll = students?.OfType<Student?>().ToList();
                        SchoolHandler.EnrollStudentInCourse(studentsToEnroll, new List<Course>(), (IUser)user);
                    }
                    break;
                case "12":
                    var studentToRemove = ValidationHelper.SelectAndValidateStudent(students);
                    if (studentToRemove != null && ValidationHelper.ValidateUser(user))
                    {
                        StudentHelper.RemoveStudent(students, studentToRemove);
                    }
                    break;
                case "13":
                    if (ValidationHelper.ValidateUser(user))
                        SchoolHandler.AssignCoursesToStudents(new List<Course>(), new List<Student?>(), (IUser)user);
                    break;
                case "14":
                    if (ValidationHelper.ValidateUser(user))
                        SchoolHandler.RecordGradesForStudents(new List<Course>(), (IUser)user);
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