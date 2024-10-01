using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Interfaces.ActionHandler;

public static class SchoolActionDemonstrator
{
    public static void DemonstrateSchoolActions(ISchoolActions schoolActions, Course course)
    {
        Console.WriteLine("Demonstrating school-specific actions:");
        schoolActions.AssignCourse(course);
        schoolActions.RemoveCourse(course);
    }
}