using SchoolManagementSystem.BusinessLogicLayer.Exceptions;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.PresentationLayer.Handlers;

public static class SchoolHandler
{
    public static void DisplayAllDetails(List<Course>? courses, List<Student?> students, List<Teacher> teachers, object user)
    {
        Exceptions.Expectations.CheckHasPermission(user, isAdmin: true);
        Exceptions.Expectations.CheckCoursesNotNull(courses);
        Exceptions.Expectations.CheckStudentsNotNull(students);
        Exceptions.Expectations.CheckTeachersNotNull(teachers);

        Console.WriteLine("Courses:");
        CourseHandler.DisplayCourseDetails(courses, user);

        Console.WriteLine("\nStudents:");
        foreach (var student in students.OfType<Student>())
        {
            StudentHandler.DisplayStudentDetails(student);
        }

        Console.WriteLine("\nTeachers:");
        foreach (var teacher in teachers)
        {
            Console.WriteLine($"Teacher ID: {teacher.GetTeacherId()}, Name: {teacher.GetTeacherFullName()}, Subject: {teacher.GetSubject()}");
        }
    }
    public static void AssignCoursesToStudents(List<Course> courses, List<Student?> students, object user)
    {
        Exceptions.Expectations.CheckHasPermission(user, isTeacherOrAdmin: true);

        foreach (var student in students)
        {
            if (student == null)
            {
                Console.WriteLine("Encountered a null student. Skipping...");
                continue;
            }

            Console.WriteLine($"Assigning courses to {student.GetStudentFullName()} (ID: {student.GetStudentId()})");

            while (true)
            {
                var course = GetCourseFromUserInput(courses);
                if (course == null) break;

                course.EnrollStudent(student);
                Console.WriteLine($"Assigned {course.GetCourseName()} to {student.GetStudentFullName()}.");
            }
        }
    }
public static void RecordGradesForStudents(List<Course> courses, object user)
{
    Exceptions.Expectations.CheckHasPermission(user, isTeacherOrAdmin: true);

    Console.WriteLine($"User {user.GetType().Name} is recording grades.");

    foreach (var course in courses)
    {
        Console.WriteLine($"Recording grades for course: {course.GetCourseName()} (ID: {course.GetCourseId()})");

        foreach (var student in course.GetEnrolledStudents().OfType<Student>())
        {
            Console.WriteLine($"Enter the grade for {student.GetStudentFullName()} (ID: {student.GetStudentId()}):");

            var input = Console.ReadLine();

            if (!double.TryParse(input, out var grade) || grade < 0 || grade > 100)
            {
                Console.WriteLine("Invalid grade. Please enter a value between 0 and 100.");
                continue;
            }

            course.AssignGrade(student, grade);
            Console.WriteLine($"Recorded grade {grade} for {student.GetStudentFullName()} in {course.GetCourseName()}.");
        }
    }
}
private static Course? GetCourseFromUserInput(List<Course> courses)
{
    Console.WriteLine("Enter the course ID to assign (or type 'done' to finish):");
    var input = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(input) || input.ToLower() == "done") return null;

    if (!int.TryParse(input, out var courseId))
    {
        Console.WriteLine("Invalid course ID. Please try again.");
        return null;
    }

    var course = courses.Find(c => c.GetCourseId() == courseId);
    if (course == null)
    {
        Console.WriteLine("Course not found. Please try again.");
    }

    return course;
}
public static void DemonstrateActions(object person, object user)
{
    Exceptions.Expectations.CheckHasPermission(user, isTeacherOrAdmin: false);

    switch (person)
    {
        case Teacher teacher:
            DemonstrateTeacherActions(teacher, user);
            break;
        case Student student:
            DemonstrateStudentActions(student, user);
            break;
        default:
            Console.WriteLine("Unknown person actions.");
            break;
    }
}
private static void DemonstrateTeacherActions(Teacher teacher, object user)
{
    Exceptions.Expectations.CheckHasPermission(user, isTeacherOrAdmin: false);

    Console.WriteLine($"Demonstrating actions for teacher: {teacher.GetTeacherFullName()} (ID: {teacher.GetTeacherId()})");
    
    teacher.Teach();
    teacher.CheckAttendance();

}
private static void DemonstrateStudentActions(Student student, object user)
{
    Exceptions.Expectations.CheckHasPermission(user, isTeacherOrAdmin: false);

    Console.WriteLine($"Demonstrating actions for student: {student.GetStudentFullName()} (ID: {student.GetStudentId()})");
    
    student.Learn();
    student.TakeTest();
    student.SubmitAssignment();
    student.Study();
    student.ParticipateInClass();
    student.AttendClass();
    student.DoHomework();
}
public static void EnrollStudentInCourse(List<Student?> students, List<Course> courses, object user)
{
    Exceptions.Expectations.CheckHasPermission(user, isTeacherOrAdmin: false);
    Exceptions.Expectations.CheckStudentsNotNull(students);
    Exceptions.Expectations.CheckCoursesNotNull(courses);

    try
    {
        var student = SelectStudent(students);
        if (student == null) return;

        var course = SelectCourse(courses);
        if (course == null) return;

        student.EnrollInCourse(course);
        Console.WriteLine($"{student.GetStudentFullName()} has been enrolled in {course.GetCourseName()}.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}
private static Student? SelectStudent(List<Student?> students)
{
    Console.WriteLine("Select a student to enroll:");
    for (var i = 0; i < students.Count; i++)
    {
        var student = students[i];
        if (student != null)
        {
            Console.WriteLine($"{i + 1}. {student.GetStudentFullName()} (ID: {student.GetStudentId()})");
        }
    }

    if (int.TryParse(Console.ReadLine(), out var studentIndex) && studentIndex >= 1 && studentIndex <= students.Count)
    {
        var selectedStudent = students[studentIndex - 1];
        if (selectedStudent != null)
        {
            return selectedStudent;
        }
    }

    Console.WriteLine("Invalid student selection.");
    return null;
}

private static Course? SelectCourse(List<Course> courses)
{
    Console.WriteLine("Select a course to enroll in:");
    for (var i = 0; i < courses.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {courses[i].GetCourseName()} (ID: {courses[i].GetCourseId()})");
    }

    if (int.TryParse(Console.ReadLine(), out var courseIndex) && courseIndex >= 1 && courseIndex <= courses.Count)
        return courses[courseIndex - 1];
    Console.WriteLine("Invalid course selection.");
    return null;
}
}
