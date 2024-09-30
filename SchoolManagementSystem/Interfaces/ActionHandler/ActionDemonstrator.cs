// File: SchoolManagementSystem/Interfaces/ActionHandler/ActionDemonstrator.cs
namespace SchoolManagementSystem.Interfaces.ActionHandler
{
    public static class ActionDemonstrator
    {
        public static void DemonstrateActions(IPersonActions person)
        {
            switch (person)
            {
                case ITeacherActions teacher:
                    DemonstrateTeacherActions(teacher);
                    break;
                case IStudentActions student:
                    DemonstrateStudentActions(student);
                    break;
                default:
                    Console.WriteLine("Unknown person actions.");
                    break;
            }
        }

        public static void DemonstrateTeacherActions(ITeacherActions teacher)
        {
            try
            {
                teacher.Teach();
                teacher.CheckAttendance();
                Console.WriteLine($"{teacher.GetTeacherFullName()} has completed teaching and attendance check.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while demonstrating teacher actions: {ex.Message}");
            }
        }

        public static void DemonstrateStudentActions(IStudentActions student)
        {
            try
            {
                student.Learn();
                student.TakeTest();
                student.SubmitAssignment();
                student.Study();
                student.ParticipateInClass();
                Console.WriteLine($"{student.GetStudentFullName()} has completed learning activities.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while demonstrating student actions: {ex.Message}");
            }
        }
    }
}