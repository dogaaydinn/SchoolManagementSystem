using SchoolManagementSystem.Interfaces;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.PresentationLayer.Handlers;

public static class SchoolHandler
{
    public static void DisplayAllDetails(List<Course>? courses, List<Student> students, List<Teacher> teachers)
    {
        Console.WriteLine("Courses:");
        CourseHandler.DisplayCourseDetails(courses);

        Console.WriteLine("\nStudents:");
        StudentHandler.DisplayStudentDetails(students, courses);

        Console.WriteLine("\nTeachers:");
        foreach (var teacher in teachers)
        {
            Console.WriteLine($"Teacher ID: {teacher!.GetTeacherId()}, Name: {teacher.GetTeacherFullName()}, Subject: {teacher.GetSubject()}");
        }
    }

    public static void AssignCoursesToStudents(List<Course>? courses, List<Student> students, IUser user)
    {
        if (user is Teacher || user is Admin)
        {
            foreach (var student in students)
            {
                Console.WriteLine($"Assigning courses to {student?.GetStudentFullName()} (ID: {student?.GetStudentId()})");

                while (true)
                {
                    Console.WriteLine("Enter the course ID to assign (or type 'done' to finish):");
                    var input = Console.ReadLine()?.Trim();

                    if (input?.ToLower() == "done") break;

                    if (!int.TryParse(input, out var courseId))
                    {
                        Console.WriteLine("Invalid course ID. Please try again.");
                        continue;
                    }

                    var course = courses?.Find(c => c.GetCourseId() == courseId);
                    if (course == null)
                    {
                        Console.WriteLine("Course not found. Please try again.");
                        continue;
                    }

                    course.EnrollStudent(student);
                    Console.WriteLine($"Assigned {course.GetCourseName()} to {student.GetStudentFullName()}.");
                }
            }
        }
        else
        {
            Console.WriteLine("You do not have permission to assign courses.");
        }
    }
    public static void RecordGradesForStudents(List<Course>? courses, IUser user)
    {
        if (user is Teacher || user is Admin)
        {
            foreach (var course in courses)
            {
                Console.WriteLine($"Recording grades for course: {course.GetCourseName()} (ID: {course.GetCourseId()})");

                foreach (var student in course.GetEnrolledStudents().OfType<Student>())
                {
                    Console.WriteLine($"Enter the grade for {student!.GetStudentFullName()} (ID: {student.GetStudentId()}):");
                    
                    if (!double.TryParse(Console.ReadLine(), out var grade) || grade < 0 || grade > 100)
                    {
                        Console.WriteLine("Invalid grade. Please enter a value between 0 and 100.");
                        continue;
                    }

                    course.AssignGrade(student, grade);
                    Console.WriteLine($"Recorded grade {grade} for {student.GetStudentFullName()} in {course.GetCourseName()}.");
                }
            }
        }
        else
        {
            Console.WriteLine("You do not have permission to record grades.");
        }
    }

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

    private static void DemonstrateTeacherActions(ITeacherActions teacher)
    {
        Console.WriteLine("Demonstrating teacher actions.");
    }

    private static void DemonstrateStudentActions(IStudentActions student)
    {
        Console.WriteLine("Demonstrating student actions.");
    }

    public static void EnrollStudentInCourse(List<Student> students, List<Course> courses, IUser user)
    {
        if (user is Teacher || user is Admin)
        {
            Console.WriteLine("Select a student to enroll:");
            for (int i = 0; i < students.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {students[i].GetStudentFullName()}");
            }
            int studentIndex = int.Parse(Console.ReadLine()) - 1;

            Console.WriteLine("Select a course to enroll in:");
            for (int i = 0; i < courses.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {courses[i].GetCourseName()}");
            }
            int courseIndex = int.Parse(Console.ReadLine()) - 1;

            students[studentIndex].EnrollInCourse(courses[courseIndex]);
        }
        else
        {
            Console.WriteLine("You do not have permission to enroll students in courses.");
        }
    }
    
    
