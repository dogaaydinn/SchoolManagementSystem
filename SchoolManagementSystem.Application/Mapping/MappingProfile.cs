using AutoMapper;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Entities;

namespace SchoolManagementSystem.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
            .ForMember(dest => dest.Roles, opt => opt.Ignore());

        // Student mappings
        CreateMap<Student, StudentDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.User.DateOfBirth))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.User.Age))
            .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src => src.User.ProfilePictureUrl));

        CreateMap<Student, StudentDetailDto>()
            .IncludeBase<Student, StudentDto>()
            .ForMember(dest => dest.Advisor, opt => opt.MapFrom(src => src.Advisor))
            .ForMember(dest => dest.EnrolledCourses, opt => opt.Ignore())
            .ForMember(dest => dest.Grades, opt => opt.Ignore());

        CreateMap<Student, StudentListDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));

        CreateMap<CreateStudentRequestDto, Student>()
            .ForMember(dest => dest.StudentNumber, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        // Teacher mappings
        CreateMap<Teacher, TeacherDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null));

        CreateMap<Teacher, TeacherDetailDto>()
            .IncludeBase<Teacher, TeacherDto>()
            .ForMember(dest => dest.Courses, opt => opt.MapFrom(src => src.Courses))
            .ForMember(dest => dest.Advisees, opt => opt.Ignore());

        CreateMap<CreateTeacherRequestDto, Teacher>()
            .ForMember(dest => dest.EmployeeNumber, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        // Course mappings
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null))
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher != null ? src.Teacher.User.FullName : null))
            .ForMember(dest => dest.SemesterName, opt => opt.MapFrom(src => src.Semester != null ? src.Semester.Name : null));

        CreateMap<Course, CourseDetailDto>()
            .IncludeBase<Course, CourseDto>()
            .ForMember(dest => dest.Teacher, opt => opt.MapFrom(src => src.Teacher))
            .ForMember(dest => dest.Prerequisites, opt => opt.Ignore())
            .ForMember(dest => dest.EnrolledStudents, opt => opt.Ignore())
            .ForMember(dest => dest.Assignments, opt => opt.MapFrom(src => src.Assignments))
            .ForMember(dest => dest.Schedules, opt => opt.MapFrom(src => src.Schedules));

        CreateMap<CreateCourseRequestDto, Course>()
            .ForMember(dest => dest.Prerequisites, opt => opt.MapFrom(src =>
                src.PrerequisiteCourseIds != null ? string.Join(",", src.PrerequisiteCourseIds) : null));

        // Grade mappings
        CreateMap<Grade, GradeDto>()
            .ForMember(dest => dest.StudentName, opt => opt.Ignore())
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.CourseName))
            .ForMember(dest => dest.CourseCode, opt => opt.MapFrom(src => src.Course.CourseCode));

        CreateMap<CreateGradeRequestDto, Grade>()
            .ForMember(dest => dest.GradedBy, opt => opt.Ignore())
            .ForMember(dest => dest.LetterGrade, opt => opt.Ignore());

        // Assignment mappings
        CreateMap<Assignment, AssignmentDto>()
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.CourseName));

        CreateMap<Assignment, AssignmentDetailDto>()
            .IncludeBase<Assignment, AssignmentDto>()
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher.User.FullName))
            .ForMember(dest => dest.Submissions, opt => opt.MapFrom(src => src.Submissions));

        CreateMap<CreateAssignmentRequestDto, Assignment>()
            .ForMember(dest => dest.TeacherId, opt => opt.Ignore());

        // Assignment Submission mappings
        CreateMap<AssignmentSubmission, AssignmentSubmissionDto>()
            .ForMember(dest => dest.StudentName, opt => opt.Ignore());

        CreateMap<SubmitAssignmentRequestDto, AssignmentSubmission>()
            .ForMember(dest => dest.StudentId, opt => opt.Ignore())
            .ForMember(dest => dest.SubmittedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Submitted"));

        // Attendance mappings
        CreateMap<Attendance, AttendanceDto>()
            .ForMember(dest => dest.StudentName, opt => opt.Ignore())
            .ForMember(dest => dest.CourseName, opt => opt.Ignore());

        CreateMap<CreateAttendanceRequestDto, Attendance>()
            .ForMember(dest => dest.MarkedBy, opt => opt.Ignore());

        // Schedule mappings
        CreateMap<Schedule, ScheduleDto>()
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.CourseName))
            .ForMember(dest => dest.CourseCode, opt => opt.MapFrom(src => src.Course.CourseCode))
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher.User.FullName));

        CreateMap<CreateScheduleRequestDto, Schedule>();

        // Department mappings
        CreateMap<Department, object>()
            .ForMember("Name", opt => opt.MapFrom(src => src.Name))
            .ForMember("Code", opt => opt.MapFrom(src => src.DepartmentCode));

        // Semester mappings
        CreateMap<Semester, object>()
            .ForMember("Name", opt => opt.MapFrom(src => src.Name))
            .ForMember("Code", opt => opt.MapFrom(src => src.Code));

        // Document mappings
        CreateMap<Document, DocumentDto>()
            .ForMember(dest => dest.UploadedByName, opt => opt.Ignore())
            .ForMember(dest => dest.FileUrl, opt => opt.Ignore());

        CreateMap<UploadDocumentRequestDto, Document>()
            .ForMember(dest => dest.FileName, opt => opt.Ignore())
            .ForMember(dest => dest.FilePath, opt => opt.Ignore())
            .ForMember(dest => dest.UploadedBy, opt => opt.Ignore());
    }
}
