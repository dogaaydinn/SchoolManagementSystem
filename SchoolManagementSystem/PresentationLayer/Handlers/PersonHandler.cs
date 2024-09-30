using SchoolManagementSystem.BusinessLogicLayer.Exceptions;
using SchoolManagementSystem.Interfaces;

namespace SchoolManagementSystem.PresentationLayer.Handlers
{
    public static class PersonHandler
    {
        public static void DemonstrateActions(IPersonActions? person)
        {
            Exceptions.Expectations.CheckPersonNotNull(person);

            Console.WriteLine("Demonstrating common actions:");
            if (person == null) return;
            person.PerformDailyRoutine();
            person.Communicate();
            person.Rest();

            DemonstrateSpecificActions(person);
        }

        private static void DemonstrateSpecificActions(IPersonActions person)
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
                    Console.WriteLine($"Unknown person type: {person.GetType().Name}.");
                    break;
            }
        }

        private static void DemonstrateTeacherActions(ITeacherActions teacher)
        {
            Console.WriteLine("Demonstrating teacher-specific actions:");
            teacher.Teach();
            teacher.CheckAttendance();
        }

        private static void DemonstrateStudentActions(IStudentActions student)
        {
            Console.WriteLine("Demonstrating student-specific actions:");
            student.Learn();
            student.TakeTest();
            student.SubmitAssignment();
            student.Study();
            student.ParticipateInClass();
            student.AttendClass();
            student.DoHomework();
        }
    }
}