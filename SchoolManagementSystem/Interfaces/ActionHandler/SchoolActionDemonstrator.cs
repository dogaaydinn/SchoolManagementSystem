// File: SchoolManagementSystem/Interfaces/ActionHandler/SchoolActionDemonstrator.cs
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.Interfaces.ActionHandler
{
    public static class SchoolActionDemonstrator
    {
        public static void DemonstrateCourseActions(ICourseManagement courseManager, Course course)
        {
            Console.WriteLine("Demonstrating course-specific actions:");
            courseManager.AssignCourse(course);
            courseManager.RemoveCourse(course);
        }

        public static void DemonstrateTeacherManagement(ITeacherManagement teacherManager, Student student, double grade)
        {
            Console.WriteLine("Demonstrating teacher-specific management actions:");
            teacherManager.AssignGrade(student, grade);
            teacherManager.ManageClassroom();
        }
    }
}