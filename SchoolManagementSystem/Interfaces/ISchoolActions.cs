using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.Interfaces;

public interface ISchoolActions 
{
    void AssignCourse(Course course);
    void RemoveCourse(Course course);
    
}