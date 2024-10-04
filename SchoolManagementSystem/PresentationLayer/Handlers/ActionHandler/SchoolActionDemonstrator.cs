using SchoolManagementSystem.Interfaces.Actions;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.PresentationLayer.Handlers.ActionHandler;

public static class SchoolActionDemonstrator
{
    public static void DemonstrateSchoolActions(ISchoolActions schoolActions, Course course)
    {
        Console.WriteLine("Demonstrating school-specific actions:");
        schoolActions.AssignCourse(course);
        schoolActions.RemoveCourse(course);
    }

    public static void DemonstrateCourseActions(List<ICourseActions> courseActionsList, List<Course?> courses, IUser user)
    {
        Console.WriteLine("Demonstrating course-specific actions:");

        foreach (var courseActions in courseActionsList)
        {
            var schoolHelper = new SchoolHelper(); 
            var course = schoolHelper.SelectCourse(courses); 
            courseActions.StartCourse(course);
            courseActions.EndCourse(course);
        }
    }
}