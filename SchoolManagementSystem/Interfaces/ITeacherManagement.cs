using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.Interfaces;

public interface ITeacherManagement
{
    void AssignGrade(Student student, double grade);
    void ManageClassroom(); 
}