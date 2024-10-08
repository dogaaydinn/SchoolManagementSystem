using Newtonsoft.Json;
using SchoolManagementSystem.BusinessLogicLayer.Authentications;
using SchoolManagementSystem.BusinessLogicLayer.Utilities;
using SchoolManagementSystem.Models.Abstract;
using SchoolManagementSystem.Models.Concrete;
using Formatting = System.Xml.Formatting;

namespace SchoolManagementSystem.Data;

public static class DataProvider
{
    private const string FilePath = "schoolMembers.json";
    private static readonly List<SchoolMember?> SchoolMembers = new();
    private static int _nextStudentId = 1;
    private static int _currentTeacherId = 26;
    private static int _currentAdminId = 21;

    static DataProvider()
    {
        if (File.Exists(FilePath))
        {
            var json = File.ReadAllText(FilePath);
            SchoolMembers = JsonConvert.DeserializeObject<List<SchoolMember>>(json) ?? new List<SchoolMember>();
        }
        else
        {
            SchoolMembers = new List<SchoolMember>();
        }
    }

    private static void SaveChanges()
    {
        var json = JsonConvert.SerializeObject(SchoolMembers, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(FilePath, json);
    }

    public static void AddSchoolMember(SchoolMember member)
    {
        SchoolMembers.Add(member);
        SaveChanges();
    }

    public static SchoolMember? GetSchoolMemberByName(string firstName, string lastName)
    {
        return SchoolMembers.FirstOrDefault(m =>
            m.GetFullName().Equals($"{firstName} {lastName}", StringComparison.OrdinalIgnoreCase));
    }
    
    public static int GenerateTeacherId()
    {
        return _currentTeacherId++;
    }

    public static int GenerateAdminId()
    {
        return _currentAdminId++;
    }
    
    public static IEnumerable<SchoolMember?> GetAllSchoolMembers()
    {
        return SchoolMembers;
    }

    public static int GenerateStudentId()
    {
        return _nextStudentId++;
    }

  public static List<Teacher> GetTeachers()
{
    var teacherList = new List<Teacher>
    {
        new("John", "Doe", new DateTime(1980, 5, 15), 1, "Mathematics", PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Jane", "Doe", new DateTime(1985, 7, 25), 2, "Physics", PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Jack", "Smith", new DateTime(1975, 9, 5), 3, "Chemistry", PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Jill", "Johnson", new DateTime(1990, 11, 20), 4, "Biology", PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("James", "Williams", new DateTime(1995, 1, 10), 5, "History", PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Jessica", "Brown", new DateTime(1992, 3, 30), 6, "Geography", PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Justin", "Lee", new DateTime(1998, 6, 15), 7, "Literature", PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Julia", "Garcia", new DateTime(2000, 8, 25), 8, "Art", PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Jacob", "Martinez", new DateTime(2002, 10, 5), 9, "Music", PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Jasmine", "Hernandez", new DateTime(2004, 12, 20), 10, "Physical Education", PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Michael", "Scott", new DateTime(1970, 3, 15), 11, "Business Studies", PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Pam", "Beesly", new DateTime(1981, 3, 25), 12, "Art", PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Jim", "Halpert", new DateTime(1978, 10, 1), 13, "Sports", PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Dwight", "Schrute", new DateTime(1975, 1, 20), 14, "Agriculture", PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Stanley", "Hudson", new DateTime(1958, 2, 19), 15, "Economics", PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Angela", "Martin", new DateTime(1971, 6, 25), 16, "Accounting", PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Kevin", "Malone", new DateTime(1973, 5, 1), 17, "Mathematics", PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Oscar", "Martinez", new DateTime(1974, 7, 4), 18, "Accounting", PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Phyllis", "Vance", new DateTime(1955, 7, 10), 19, "Home Economics", PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Meredith", "Palmer", new DateTime(1960, 12, 11), 20, "Health Education", PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword()))
    };
    
    foreach (var teacher in teacherList)
    {
        var existingTeacher = SchoolMembers.OfType<Teacher>().FirstOrDefault(t => t.Id == teacher.Id);
        if (existingTeacher == null)
        {
            SchoolMembers.Add(teacher);
        }
        else
        {
            teacher.SetPassword(existingTeacher.Password);
        }
    }

    SchoolMembers.AddRange(teacherList);

    return teacherList;
}

public static List<Student> GetStudents()
{
    var studentList = new List<Student>
    {
        new("Alice", "Smith", new DateTime(2005, 3, 22), 101, 3.8, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Bob", "Johnson", new DateTime(2004, 7, 19), 102, 3.2, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Charlie", "Brown", new DateTime(2006, 9, 11), 103, 3.9, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("David", "Lee", new DateTime(2003, 11, 30), 104, 3.5, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Eve", "Williams", new DateTime(2007, 1, 25), 105, 3.7, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Frank", "Davis", new DateTime(2002, 5, 5), 106, 3.6, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Grace", "Rodriguez", new DateTime(2008, 8, 15), 107, 3.4, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Henry", "Martinez", new DateTime(2001, 10, 10), 108, 3.3, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Isabel", "Hernandez", new DateTime(2009, 12, 1), 109, 3.1, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Kevin", "Garcia", new DateTime(2000, 2, 20), 110, 3.0, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Linda", "Lopez", new DateTime(2010, 4, 10), 111, 2.9, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Michael", "Perez", new DateTime(1999, 6, 5), 112, 2.8, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Nancy", "Torres", new DateTime(2011, 7, 30), 113, 2.7, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Olivia", "Adams", new DateTime(2005, 2, 14), 114, 3.6, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Paul", "Baker", new DateTime(2004, 5, 21), 115, 3.4, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Quincy", "Clark", new DateTime(2006, 8, 9), 116, 3.5, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Rachel", "Davis", new DateTime(2003, 12, 3), 117, 3.8, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Sam", "Evans", new DateTime(2007, 4, 17), 118, 3.2, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Tina", "Foster", new DateTime(2002, 6, 25), 119, 3.9, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Uma", "Green", new DateTime(2008, 9, 13), 120, 3.1, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Victor", "Harris", new DateTime(2001, 11, 7), 121, 3.0, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Wendy", "Iverson", new DateTime(2009, 1, 19), 122, 2.9, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Xander", "Jackson", new DateTime(2000, 3, 11), 123, 2.8, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Yara", "King", new DateTime(2010, 5, 23), 124, 2.7, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword())),
        new("Zane", "Lewis", new DateTime(2011, 7, 15), 125, 2.6, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword()))
    };
    
    foreach (var student in studentList)
    {
        var existingStudent = SchoolMembers.OfType<Student>().FirstOrDefault(s => s.Id == student.Id);
        if (existingStudent == null)
        {
            SchoolMembers.Add(student);
        }
        else
        {
            student.SetPassword(existingStudent.Password);
        }
    }

    SchoolMembers.AddRange(studentList);

    return studentList;
}
    public static List<Course> GetCourses()
    {
        var teachers = GetTeachers();

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

        return courses;
    }

    public static List<Admin> GetAdmins()
    {
        var adminList = new List<Admin>
        {
            new("Admin", "Admin", new DateTime(1990, 1, 1), 1000, PasswordHelper.HashPassword(Authenticator.GenerateRandomPassword()))
        };

        foreach (var admin in adminList)
        {
            var existingAdmin = SchoolMembers.OfType<Admin>().FirstOrDefault(a => a.Id == admin.Id);
            if (existingAdmin == null)
            {
                SchoolMembers.Add(admin);
            }
            else
            {
                admin.SetPassword(existingAdmin.GetPassword());
            }
        }

        SchoolMembers.AddRange(adminList);

        return adminList;
    }
}