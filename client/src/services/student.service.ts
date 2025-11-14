import apiClient from './api';
import {
  StudentDto,
  CreateStudentRequest,
  UpdateStudentRequest,
  PaginatedResponse,
  StudentQueryParams,
  EnrollStudentRequest,
  EnrollmentDto,
  GradeDto,
} from '@/types/api';

export class StudentService {
  private readonly BASE_PATH = '/students';

  async getStudents(params?: StudentQueryParams): Promise<PaginatedResponse<StudentDto>> {
    const response = await apiClient.get<PaginatedResponse<StudentDto>>(this.BASE_PATH, {
      params,
    });
    return response.data!;
  }

  async getStudentById(id: number): Promise<StudentDto> {
    const response = await apiClient.get<StudentDto>(`${this.BASE_PATH}/${id}`);
    return response.data!;
  }

  async createStudent(data: CreateStudentRequest): Promise<StudentDto> {
    const response = await apiClient.post<StudentDto, CreateStudentRequest>(
      this.BASE_PATH,
      data
    );
    return response.data!;
  }

  async updateStudent(id: number, data: UpdateStudentRequest): Promise<StudentDto> {
    const response = await apiClient.put<StudentDto, UpdateStudentRequest>(
      `${this.BASE_PATH}/${id}`,
      data
    );
    return response.data!;
  }

  async deleteStudent(id: number): Promise<void> {
    await apiClient.delete(`${this.BASE_PATH}/${id}`);
  }

  async enrollStudent(request: EnrollStudentRequest): Promise<EnrollmentDto> {
    const response = await apiClient.post<EnrollmentDto, EnrollStudentRequest>(
      `${this.BASE_PATH}/${request.studentId}/enroll`,
      { courseId: request.courseId }
    );
    return response.data!;
  }

  async unenrollStudent(studentId: number, courseId: number): Promise<void> {
    await apiClient.delete(`${this.BASE_PATH}/${studentId}/unenroll/${courseId}`);
  }

  async getStudentCourses(studentId: number): Promise<EnrollmentDto[]> {
    const response = await apiClient.get<EnrollmentDto[]>(
      `${this.BASE_PATH}/${studentId}/courses`
    );
    return response.data!;
  }

  async getStudentGrades(studentId: number): Promise<GradeDto[]> {
    const response = await apiClient.get<GradeDto[]>(`${this.BASE_PATH}/${studentId}/grades`);
    return response.data!;
  }

  async getStudentTranscript(studentId: number): Promise<Blob> {
    const response = await apiClient.get(`${this.BASE_PATH}/${studentId}/transcript`, {
      responseType: 'blob',
    });
    return response.data as unknown as Blob;
  }

  async exportStudents(params?: StudentQueryParams): Promise<Blob> {
    const response = await apiClient.get(`${this.BASE_PATH}/export`, {
      params,
      responseType: 'blob',
    });
    return response.data as unknown as Blob;
  }
}

export const studentService = new StudentService();
export default studentService;
