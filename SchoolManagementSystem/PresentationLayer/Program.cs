using Microsoft.Extensions.DependencyInjection;
using SchoolManagementSystem.BusinessLogicLayer.Validations;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Interfaces;
using SchoolManagementSystem.Interfaces.Actions;
using SchoolManagementSystem.Interfaces.Helper;
using SchoolManagementSystem.Interfaces.Validation;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;
using SchoolManagementSystem.PresentationLayer.Handlers.ActionHandler;

namespace SchoolManagementSystem.PresentationLayer;

internal static class Program
{
    private static void Main(string[] args)
    {
        // Initialize data with required parameters
        List<Teacher?>? teachers = DataProvider.GetTeachers(10, "example");
        List<Student> students = DataProvider.GetStudents(20, "example");
        List<Course>? courses = DataProvider.GetCourses(teachers, "example");

        // Display the main menu
        MenuHandler.DisplayMainMenu(students, courses, teachers);
        
        Admin admin = new Admin("John", "Doe", new DateTime(1980, 1, 1), 12345);
        admin.DisplayUserInfo();
        
        // Demonstrate school actions
        ISchoolActions school = new School(); // Assuming you have a School class that implements ISchoolActions
        Course course = courses[0]; // Example course
        SchoolActionDemonstrator.DemonstrateSchoolActions(school, course);
        
        var services = new ServiceCollection();
        services.AddSingleton<IValidationHelper, ValidationHelper>();
        services.AddSingleton<ICourseHandler, CourseHandler>();
        services.AddSingleton<IStudentHandler, StudentHandler>();
        services.AddSingleton<ISchoolHelper, SchoolHelper>();
        services.AddSingleton<SchoolHandler>();

        var serviceProvider = services.BuildServiceProvider();
        var schoolHandler = serviceProvider.GetService<SchoolHandler>();
        
        IStudentHelper studentHelper = new StudentHelper();
        studentHelper.DisplayStudentInfo(student);
        
        IPersonHelper personHelper = new PersonHelper();
        personHelper.CheckAndDemonstrateActions(somePerson, someUser);

        
        ICourseHelper courseHelper = new CourseHelper();
        var courseById = courseHelper.GetCourseById(someCourses);
        var courseByName = courseHelper.GetCourseByName(someCourses);
        
        ISchoolHelper schoolHelper = new SchoolHelper();
        var courseFromUserInput = schoolHelper.GetCourseFromUserInput(someCourses);
        var validGrade = schoolHelper.GetValidGrade(someStudent);
        var selectedStudent = schoolHelper.SelectStudent(someStudents);
        var selectedCourse = schoolHelper.SelectCourse(someCourses);
        schoolHelper.DisplayStudents(someStudents);
        
        ITeacherHelper teacherHelper = new TeacherHelper();
        teacherHelper.DisplayTeacherInfo(someTeacher);
        




    }
}