using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;

namespace SchoolManagementSystem.PresentationLayer.Menu;

public static class TeacherMenu
{
    public static void DisplayTeacherMenu(List<Teacher> teachers, List<Student> students)
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

            switch (choice)
            {
                case "1":
                    SchoolHandler.DemonstrateActions(teachers[0]);
                    break;
                case "2":
                    SchoolHandler.EnrollStudentInCourse(teachers, students);
                case "3":
                    SchoolHandler.AssignCoursesToStudents(teachers, students);
                    break;
                case "4":
                    SchoolHandler.RecordGradesForStudents(teachers, students);
                    break;
                case "5":
                    SchoolHandler.DisplayAllDetails(teachers, students);
                    break;
                case "6":
                    StudentHandler.AssignGradeToStudent(teachers, students);
                    break;
                case "7":
                    .UpdateStudentGPA(students);
                    break;
                case "8":
                    TeacherHandler.DisplayStudentNames(students);
                    break;
                case "9":
                    TeacherHandler.DisplayTeacherDetails(teachers);
                    break;
                case "10":
                    TeacherHandler.DisplayTeacherNames(teachers);
                    break;
                case "11":
                    TeacherHandler.UpdateTeacherDetails(teachers);
                    break;
                case "12":
                    TeacherHandler.ListStudentsInCourses(teachers);
                    break;
                case "13":
                    TeacherHandler.DemonstrateTeacherMethods(teachers);
                    break;
                case "14":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}