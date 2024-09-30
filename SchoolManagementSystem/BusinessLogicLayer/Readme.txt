Business Logic Layer (BLL): İş kurallarının ve operasyonların olduğu katman. Öğrenci kaydetme, öğretmen atama gibi işlemler burada yapılır.


1. Classes and Objects
Person sınıfı: Öğrenci ve öğretmenler için ortak özellikleri tutan temel sınıf olacak.
Properties: FirstName, LastName, DateOfBirth
Method: GetFullName() → Adı ve soyadı birleştirip tam adını döndüren bir metot.
Student sınıfı: Person sınıfından türeyecek.
Additional Properties: StudentId, GPA
Method: DisplayStudentDetails() → Öğrencinin detaylarını ekrana yazdıran metot.
Teacher sınıfı: Person sınıfından türeyecek.
Additional Properties: TeacherId, Subject
Method: DisplayTeacherDetails() → Öğretmenin detaylarını ekrana yazdıran metot.
Course sınıfı: Her ders için öğretmen ve öğrencileri yönetecek.
Properties: CourseId, CourseName, Teacher, List<Student> EnrolledStudents
Methods:
EnrollStudent(Student student) → Öğrenciyi derse kaydeden metot.
ListStudents() → Derse kayıtlı öğrencileri listeleyen metot.
AssignTeacher(Teacher teacher) → Derse öğretmen atayan metot.



Class Person:
    Private: FirstName, LastName, DateOfBirth
    Method: GetFullName()
        Return FirstName + " " + LastName

Class Student Inherits Person:
    Private: StudentId, GPA
    Method: DisplayStudentDetails()
        Print Student details (Full Name, StudentId, GPA)

Class Teacher Inherits Person:
    Private: TeacherId, Subject
    Method: DisplayTeacherDetails()
        Print Teacher details (Full Name, TeacherId, Subject)

Class Course:
    Private: CourseId, CourseName, Teacher, EnrolledStudents (List of Students)
    Method: EnrollStudent(student)
        Add student to EnrolledStudents
    Method: ListStudents()
        Print all students enrolled in the course
    Method: AssignTeacher(teacher)
        Set the teacher for the course


2. Encapsulation
Tüm sınıf özelliklerini (FirstName, LastName, vs.) private yap. Dışarıdan erişimi sadece public getter ve setter metodlarıyla sağla.
Pseudocode:
vbnet
Kodu kopyala
Class Person:
    Private: FirstName, LastName, DateOfBirth
    Public Getter/Setter for FirstName, LastName, DateOfBirth
    
    
    
    
    
    
    course-student 
    exceptionları metotlara ekle
    demonstrate school action not use
    school action-teacher action
    user interface
    school member displaydetail not used
    student ve course kullanılmayan metotlar