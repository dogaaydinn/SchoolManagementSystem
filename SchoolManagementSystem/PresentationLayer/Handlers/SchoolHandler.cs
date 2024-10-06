using SchoolManagementSystem.BusinessLogicLayer.Validations;
using SchoolManagementSystem.Interfaces.Helper;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Interfaces.Validation;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers.ActionHandler;

namespace SchoolManagementSystem.PresentationLayer.Handlers;

public static class SchoolHandler
{
    public static void DisplayAllDetails(ISchoolHelper schoolHelper, List<Course> courses, List<Student?>? students, List<Teacher?>? teachers, object? user)
    {
        ValidationHelper.ValidateUserAndEntities(user as IUser, true, courses, students, teachers);
        Console.WriteLine("Courses:");
        schoolHelper.DisplayCourses(courses);

        Console.WriteLine("\nStudents:");
        schoolHelper.DisplayStudents(students);

        Console.WriteLine("\nTeachers:");
        DisplayTeacherDetails(teachers);
    }

    public static void AssignCoursesToStudents(ISchoolHelper schoolHelper, List<Course> courses, List<Student?> students, object user)
    {
        ValidationHelper.ValidateUserAndEntities(user as IUser, true, courses, students);

        foreach (var student in students.OfType<Student>()) AssignCoursesToStudent(schoolHelper, student, courses);
    }

    public static void RecordGradesForStudents(ISchoolHelper schoolHelper, List<Course> courses, object user)
    {
        ValidationHelper.ValidateUserAndEntities(user as IUser, true, courses);

        foreach (var course in courses)
        {
            RecordGradesForCourse(schoolHelper, course);
        }
    }
    public static void EnrollStudentInCourse(List<Course?>? courses, List<Student?> students, IUser user)
    {
        ValidationHelper.ValidateNotNull(courses, "Courses list cannot be null.");
        ValidationHelper.ValidateNotNull(students, "Students list cannot be null.");
        ValidationHelper.ValidateNotNull(user, "User cannot be null.");

        foreach (var course in courses)
        {
            if (course == null)
            {
                Console.WriteLine("Course cannot be null. Skipping...");
                continue;
            }

            Console.WriteLine($"Enrolling students in course: {course.GetCourseName()} (ID: {course.GetCourseId()})");
            foreach (var student in students.OfType<Student>())
            {
                course.EnrollStudent(student);
                Console.WriteLine(
                    $"  Enrolled Student ID: {student.GetStudentId()}, Name: {student.GetStudentFullName()}");
            }
        }
    }

    public static void DemonstrateActions(object person, object user)
    {
        ValidationHelper.CheckHasPermission(user as IUser, true);
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
    
    private static void AssignCoursesToStudent(ISchoolHelper schoolHelper, Student student, List<Course> courses)
    {
        Console.WriteLine($"Assigning courses to {student.GetStudentFullName()} (ID: {student.GetStudentId()})");

        while (true)
        {
            var course = schoolHelper.GetCourseFromUserInput(courses);
            if (course == null) break;

            course.EnrollStudent(student);
            Console.WriteLine($"Assigned {course.GetCourseName()} to {student.GetStudentFullName()}.");
        }
    }

    private static void RecordGradesForCourse(ISchoolHelper schoolHelper, Course course)
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
            var grade = schoolHelper.GetValidGrade(student);
            if (grade == null) continue;
            course.AssignGrade(student, grade.Value);
            Console.WriteLine(
                $"Recorded grade {grade} for {student.GetStudentFullName()} in {course.GetCourseName()}.");
        }
    }

    private static void DisplayTeacherDetails(List<Teacher?> teachers)
    {
        foreach (var teacher in teachers.OfType<Teacher>())
            Console.WriteLine(
                $"Teacher ID: {teacher.GetTeacherId()}, Name: {teacher.GetTeacherFullName()}, Subject: {teacher.GetSubject()}");
    }
}