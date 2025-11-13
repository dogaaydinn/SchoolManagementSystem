// API Response Types
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data?: T;
  errors?: Record<string, string[]>;
  statusCode: number;
  timestamp: string;
}

export interface PaginatedResponse<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalCount: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

// Authentication Types
export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
  role: UserRole;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  expiresAt: string;
  user: UserDto;
}

export interface UserDto {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  role: UserRole;
  isActive: boolean;
  createdAt: string;
  lastLoginAt?: string;
}

export enum UserRole {
  SuperAdmin = 'SuperAdmin',
  Admin = 'Admin',
  Teacher = 'Teacher',
  Student = 'Student',
  Parent = 'Parent'
}

// Student Types
export interface StudentDto {
  id: number;
  studentNumber: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  phoneNumber?: string;
  dateOfBirth: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  enrollmentDate: string;
  gpa?: number;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateStudentRequest {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  dateOfBirth: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  enrollmentDate: string;
}

export interface UpdateStudentRequest extends Partial<CreateStudentRequest> {
  isActive?: boolean;
}

// Course Types
export interface CourseDto {
  id: number;
  courseCode: string;
  title: string;
  description?: string;
  credits: number;
  maxStudents: number;
  currentEnrollment: number;
  departmentId?: number;
  departmentName?: string;
  teacherId?: number;
  teacherName?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateCourseRequest {
  courseCode: string;
  title: string;
  description?: string;
  credits: number;
  maxStudents: number;
  departmentId?: number;
  teacherId?: number;
}

export interface UpdateCourseRequest extends Partial<CreateCourseRequest> {
  isActive?: boolean;
}

// Grade Types
export interface GradeDto {
  id: number;
  studentId: number;
  studentName: string;
  courseId: number;
  courseName: string;
  assignmentId?: number;
  assignmentName?: string;
  value: number;
  maxValue: number;
  percentage: number;
  letterGrade: string;
  gradedBy: string;
  gradedAt: string;
  feedback?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateGradeRequest {
  studentId: number;
  courseId: number;
  assignmentId?: number;
  value: number;
  maxValue: number;
  feedback?: string;
}

export interface GradeDistribution {
  letterGrade: string;
  count: number;
  percentage: number;
}

// Enrollment Types
export interface EnrollmentDto {
  id: number;
  studentId: number;
  studentName: string;
  courseId: number;
  courseName: string;
  enrollmentDate: string;
  status: EnrollmentStatus;
  grade?: number;
  letterGrade?: string;
}

export enum EnrollmentStatus {
  Active = 'Active',
  Completed = 'Completed',
  Dropped = 'Dropped',
  Withdrawn = 'Withdrawn'
}

export interface EnrollStudentRequest {
  studentId: number;
  courseId: number;
}

// Teacher Types
export interface TeacherDto {
  id: number;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  phoneNumber?: string;
  departmentId?: number;
  departmentName?: string;
  specialization?: string;
  hireDate: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateTeacherRequest {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  departmentId?: number;
  specialization?: string;
  hireDate: string;
}

// Assignment Types
export interface AssignmentDto {
  id: number;
  courseId: number;
  courseName: string;
  title: string;
  description?: string;
  dueDate: string;
  maxPoints: number;
  type: AssignmentType;
  isPublished: boolean;
  createdAt: string;
  updatedAt: string;
}

export enum AssignmentType {
  Homework = 'Homework',
  Quiz = 'Quiz',
  Exam = 'Exam',
  Project = 'Project',
  Lab = 'Lab'
}

export interface CreateAssignmentRequest {
  courseId: number;
  title: string;
  description?: string;
  dueDate: string;
  maxPoints: number;
  type: AssignmentType;
}

// Attendance Types
export interface AttendanceDto {
  id: number;
  studentId: number;
  studentName: string;
  courseId: number;
  courseName: string;
  date: string;
  status: AttendanceStatus;
  notes?: string;
  recordedBy: string;
  createdAt: string;
}

export enum AttendanceStatus {
  Present = 'Present',
  Absent = 'Absent',
  Late = 'Late',
  Excused = 'Excused'
}

// Dashboard Types
export interface DashboardStats {
  totalStudents: number;
  totalCourses: number;
  totalTeachers: number;
  activeEnrollments: number;
  averageGPA: number;
  attendanceRate: number;
}

export interface RecentActivity {
  id: string;
  type: ActivityType;
  message: string;
  timestamp: string;
  userId?: string;
  userName?: string;
}

export enum ActivityType {
  StudentEnrolled = 'StudentEnrolled',
  GradeSubmitted = 'GradeSubmitted',
  AttendanceRecorded = 'AttendanceRecorded',
  AssignmentCreated = 'AssignmentCreated',
  CourseCreated = 'CourseCreated'
}

// Query Parameters
export interface QueryParams {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}

export interface StudentQueryParams extends QueryParams {
  isActive?: boolean;
  enrollmentDateFrom?: string;
  enrollmentDateTo?: string;
}

export interface CourseQueryParams extends QueryParams {
  isActive?: boolean;
  departmentId?: number;
  teacherId?: number;
}

export interface GradeQueryParams extends QueryParams {
  studentId?: number;
  courseId?: number;
  assignmentId?: number;
  fromDate?: string;
  toDate?: string;
}
