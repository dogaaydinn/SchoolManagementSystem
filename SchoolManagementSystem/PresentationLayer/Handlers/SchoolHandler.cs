using SchoolManagementSystem.Interfaces.Helper;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Interfaces.Validation;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers.ActionHandler;

namespace SchoolManagementSystem.PresentationLayer.Handlers;

public class SchoolHandler
{
    private static ISchoolHelper _schoolHelper;
    private static IValidationHelper _validationHelper;

    public SchoolHandler(ISchoolHelper schoolHelper, IValidationHelper validationHelper)
    {
        _schoolHelper = schoolHelper;
        _validationHelper = validationHelper;
    }

    public static void DisplayAllDetails(List<Course>? courses, List<Student?>? students, List<Teacher?>? teachers, object? user)
    {
        ValidateUserAndEntities(user as IUser, true, courses, students, teachers);
        Console.WriteLine("Courses:");
        _schoolHelper.DisplayCourses(courses ?? new List<Course>());

        Console.WriteLine("\nStudents:");
        _schoolHelper.DisplayStudents(students ?? new List<Student?>());

        Console.WriteLine("\nTeachers:");
        DisplayTeacherDetails(teachers ?? new List<Teacher?>());
    }

    public static void AssignCoursesToStudents(List<Course>? courses, List<Student?> students, object? user)
    {
        ValidateUserAndEntities(user as IUser, true, courses, students);

        foreach (var student in students.OfType<Student>())
        {
            AssignCoursesToStudent(student, courses);
        }
    }

    public static void RecordGradesForStudents(List<Course>? courses, object? user)
    {
        ValidateUserAndEntities(user as IUser, true, courses);

        foreach (var course in courses ?? Enumerable.Empty<Course>())
        {
            RecordGradesForCourse(course);
        }
    }

    public static void EnrollStudentInCourse(List<Student?>? students, List<Course>? courses, object? user)
    {
        ValidateUserAndEntities(user as IUser, false, students, courses);

        var student = _schoolHelper.SelectStudent(students);
        if (student == null) return;

        var course = _schoolHelper.SelectCourse(courses);
        if (course == null) return;

        student.EnrollInCourse(course);
        Console.WriteLine($"{student.GetStudentFullName()} has been enrolled in {course.GetCourseName()}.");
    }

    public static void DemonstrateActions(object? person, object? user)
    {
        _validationHelper.CheckHasPermission(user as IUser, true);
        switch (person)
        {
            case Teacher teacher:
                PersonActionDemonstrator.DemonstrateTeacherActions(teacher);
                break;
            case Student student:
                PersonActionDemonstrator.DemonstrateStudentActions(student);
                break;
            default:
                Console.WriteLine("Unknown person actions.");
                break;
        }
    }

    private static void ValidateUserAndEntities(IUser? user, bool isAdmin, params object?[] entities)
    {
        _validationHelper.CheckHasPermission(user, isAdmin);
        foreach (var entity in entities)
        {
            switch (entity)
            {
                case List<object> entityList:
                    _validationHelper.ValidateNotNull(entityList, entity != null ? $"{entity.GetType().Name} cannot be null." : "Entity cannot be null.");
                    break;
                case List<Course> courseList:
                    _validationHelper.ValidateNotNull(courseList, "Course list cannot be null.");
                    break;
                case List<Student?> studentList:
                    _validationHelper.ValidateNotNull(studentList, "Student list cannot be null.");
                    break;
                case List<Teacher?> teacherList:
                    _validationHelper.ValidateNotNull(teacherList, "Teacher list cannot be null.");
                    break;
                default:
                    throw new ArgumentException("Entity is not a valid list.");
            }
        }
    }

    private static void AssignCoursesToStudent(Student student, List<Course>? courses)
    {
        Console.WriteLine($"Assigning courses to {student.GetStudentFullName()} (ID: {student.GetStudentId()})");

        while (true)
        {
            var course = _schoolHelper.GetCourseFromUserInput(courses);
            if (course == null) break;

            course.EnrollStudent(student);
            Console.WriteLine($"Assigned {course.GetCourseName()} to {student.GetStudentFullName()}.");
        }
    }

    private static void RecordGradesForCourse(Course course)
    {
        Console.WriteLine($"Recording grades for course: {course.GetCourseName()} (ID: {course.GetCourseId()})");

        var enrolledStudents = course.GetEnrolledStudents();
        if (enrolledStudents == null || !enrolledStudents.Any())
        {
            Console.WriteLine("No students enrolled in this course.");
            return;
        }

        foreach (var student in enrolledStudents.OfType<Student>())
        {
            var grade = _schoolHelper.GetValidGrade(student);
            if (grade == null) continue;
            course.AssignGrade(student, grade.Value);
            Console.WriteLine($"Recorded grade {grade} for {student.GetStudentFullName()} in {course.GetCourseName()}.");
        }
    }

    private static void DisplayTeacherDetails(List<Teacher?> teachers)
    {
        foreach (var teacher in teachers.OfType<Teacher>())
        {
            Console.WriteLine($"Teacher ID: {teacher.GetTeacherId()}, Name: {teacher.GetTeacherFullName()}, Subject: {teacher.GetSubject()}");
        }
    }
    
}