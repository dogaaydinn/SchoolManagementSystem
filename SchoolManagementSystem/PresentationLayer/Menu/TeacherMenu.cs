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
            Console.WriteLine("13. Exit");
            Console.Write("Enter your choice: ");

            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    TeacherHandler.DemonstrateActions(teachers);
                    break;
                case "2":
                    TeacherHandler.EnrollStudentInCourse(teachers, students);
                    break;
                case "3":
                    TeacherHandler.AssignCoursesToStudents(teachers, students);
                    break;
                case "4":
                    TeacherHandler.RecordGradesForStudents(teachers, students);
                    break;
                case "5":
                    TeacherHandler.DisplayAllDetails(teachers, students);
                    break;
                case "6":
                    TeacherHandler.AssignGradeToStudent(teachers, students);
                    break;
                case "7":
                    TeacherHandler.UpdateStudentGPA(students);
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
                    TeacherHandler.UpdateTeacherDetails(teachers);
                    break;
                case "13":
                    TeacherHandler.ListStudentsInCourses(teachers);
                    break;
                case "14":
                    TeacherHandler.DemonstrateTeacherMethods(teacher);
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