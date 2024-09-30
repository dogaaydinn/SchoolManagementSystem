using System.Diagnostics;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.Data;

public static class DataProvider
{
    public static List<Teacher> GetTeachers(List<Course> courses)
    {
        var teacherList = new List<Teacher>
        {
            new("John", "Doe", new DateTime(1980, 5, 15), 1, "Mathematics"),
            new("Jane", "Doe", new DateTime(1985, 7, 25), 2, "Physics"),
            new("Jack", "Smith", new DateTime(1975, 9, 5), 3, "Chemistry"),
            new("Jill", "Johnson", new DateTime(1990, 11, 20), 4, "Biology"),
            new("James", "Williams", new DateTime(1995, 1, 10), 5, "History"),
            new("Jessica", "Brown", new DateTime(1992, 3, 30), 6, "Geography"),
            new("Justin", "Lee", new DateTime(1998, 6, 15), 7, "Literature"),
            new("Julia", "Garcia", new DateTime(2000, 8, 25), 8, "Art"),
            new("Jacob", "Martinez", new DateTime(2002, 10, 5), 9, "Music"),
            new("Jasmine", "Hernandez", new DateTime(2004, 12, 20), 10, "Physical Education"),
            new("Michael", "Scott", new DateTime(1970, 3, 15), 11, "Business Studies"),
            new("Pam", "Beesly", new DateTime(1981, 3, 25), 12, "Art"),
            new("Jim", "Halpert", new DateTime(1978, 10, 1), 13, "Sports"),
            new("Dwight", "Schrute", new DateTime(1975, 1, 20), 14, "Agriculture"),
            new("Stanley", "Hudson", new DateTime(1958, 2, 19), 15, "Economics"),
            new("Angela", "Martin", new DateTime(1971, 6, 25), 16, "Accounting"),
            new("Kevin", "Malone", new DateTime(1973, 5, 1), 17, "Mathematics"),
            new("Oscar", "Martinez", new DateTime(1974, 7, 4), 18, "Accounting"),
            new("Phyllis", "Vance", new DateTime(1955, 7, 10), 19, "Home Economics"),
            new("Meredith", "Palmer", new DateTime(1960, 12, 11), 20, "Health Education")
        };

        foreach (var teacher in teacherList)
        {
            var assignedCourses = courses.Where(c => c.GetAssignedTeacherName().Contains(teacher.GetTeacherFullName())).ToList();
            var averageGpa = assignedCourses.Any() ? assignedCourses.Average(c => c.GetEnrolledStudents().Average(s =>
            {
                Debug.Assert(s != null, nameof(s) + " != null");
                return s.GetGpa();
            })) : 0.0;
            var studentCount = assignedCourses.Sum(c => c.GetEnrolledStudents().Count);

            Console.WriteLine($"Teacher: {teacher.GetTeacherFullName()}, Average GPA: {averageGpa:F2}, Number of Students: {studentCount}");
        }

        return teacherList;
    }
    
    public static List<Student> GetStudents(List<Course> courses)
    {
        var studentList = new List<Student>
        {
            new("Alice", "Smith", new DateTime(2005, 3, 22), 101, 3.8),
            new("Bob", "Johnson", new DateTime(2004, 7, 19), 102, 3.2),
            new("Charlie", "Brown", new DateTime(2006, 9, 11), 103, 3.9),
            new("David", "Lee", new DateTime(2003, 11, 30), 104, 3.5),
            new("Eve", "Williams", new DateTime(2007, 1, 25), 105, 3.7),
            new("Frank", "Davis", new DateTime(2002, 5, 5), 106, 3.6),
            new("Grace", "Rodriguez", new DateTime(2008, 8, 15), 107, 3.4),
            new("Henry", "Martinez", new DateTime(2001, 10, 10), 108, 3.3),
            new("Isabel", "Hernandez", new DateTime(2009, 12, 1), 109, 3.1),
            new("Kevin", "Garcia", new DateTime(2000, 2, 20), 110, 3.0),
            new("Linda", "Lopez", new DateTime(2010, 4, 10), 111, 2.9),
            new("Michael", "Perez", new DateTime(1999, 6, 5), 112, 2.8),
            new("Nancy", "Torres", new DateTime(2011, 7, 30), 113, 2.7),
            new("Olivia", "Adams", new DateTime(2005, 2, 14), 114, 3.6),
            new("Paul", "Baker", new DateTime(2004, 5, 21), 115, 3.4),
            new("Quincy", "Clark", new DateTime(2006, 8, 9), 116, 3.5),
            new("Rachel", "Davis", new DateTime(2003, 12, 3), 117, 3.8),
            new("Sam", "Evans", new DateTime(2007, 4, 17), 118, 3.2),
            new("Tina", "Foster", new DateTime(2002, 6, 25), 119, 3.9),
            new("Uma", "Green", new DateTime(2008, 9, 13), 120, 3.1),
            new("Victor", "Harris", new DateTime(2001, 11, 7), 121, 3.0),
            new("Wendy", "Iverson", new DateTime(2009, 1, 19), 122, 2.9),
            new("Xander", "Jackson", new DateTime(2000, 3, 11), 123, 2.8),
            new("Yara", "King", new DateTime(2010, 5, 23), 124, 2.7),
            new("Zane", "Lewis", new DateTime(2011, 7, 15), 125, 2.6)
        };

        foreach (var student in studentList)
        {
            var enrolledCourses = courses.Where(c => c.IsStudentEnrolled(student)).ToList();
            var averageGpa = enrolledCourses.Any() ? enrolledCourses.Average(c => c.GetAssignedGrades(student)) : 0.0;
            var courseCount = enrolledCourses.Count;

            Console.WriteLine($"Student: {student.GetStudentFullName()}, Average GPA: {averageGpa:F2}, Number of Courses: {courseCount}");
        }

        return studentList;
    }

    public static List<Course> GetCourses(List<Teacher> teachers, List<Student> students)
    {
        if (teachers == null || teachers.Count == 0)
            throw new ArgumentException("Teacher list cannot be null or empty.");

        var courses = new List<Course>
        {
            new(1, "Algebra", teachers[0], 3),
            new(2, "Physics", teachers[1], 4),
            new(3, "Chemistry", teachers[2], 4),
            new(4, "Biology", teachers[3], 3),
            new(5, "History", teachers[4], 3),
            new(6, "Geography", teachers[5], 2),
            new(7, "Literature", teachers[6], 3),
            new(8, "Art", teachers[7], 2),
            new(9, "Music", teachers[8], 2),
            new(10, "Physical Education", teachers[9], 1),
            new(11, "Business Studies", teachers[10], 3),
            new(12, "Advanced Art", teachers[11], 2),
            new(13, "Sports Management", teachers[12], 3),
            new(14, "Agricultural Science", teachers[13], 4),
            new(15, "Economics", teachers[14], 3),
            new(16, "Accounting", teachers[15], 3),
            new(17, "Advanced Mathematics", teachers[16], 4),
            new(18, "Financial Accounting", teachers[17], 3),
            new(19, "Home Economics", teachers[18], 2),
            new(20, "Health Education", teachers[19], 2)
        };

        foreach (var course in courses)
        {
            var enrolledStudents = students.Where(s => s.IsEnrolledInCourse(course.GetCourseId())).ToList();
            var averageGpa = enrolledStudents.Any() ? enrolledStudents.Average(s => s.GetGpa()) : 0.0;
            var studentCount = enrolledStudents.Count;

            Console.WriteLine($"Course: {course.GetCourseName()}, Average GPA: {averageGpa:F2}, Number of Students: {studentCount}");
        }

        return courses;
    }
}