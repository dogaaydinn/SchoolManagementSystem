import apiClient from './api';
import {
  CourseDto,
  CreateCourseRequest,
  UpdateCourseRequest,
  PaginatedResponse,
  CourseQueryParams,
  StudentDto,
  AssignmentDto,
} from '@/types/api';

export class CourseService {
  private readonly BASE_PATH = '/courses';

  async getCourses(params?: CourseQueryParams): Promise<PaginatedResponse<CourseDto>> {
    const response = await apiClient.get<PaginatedResponse<CourseDto>>(this.BASE_PATH, {
      params,
    });
    return response.data!;
  }

  async getCourseById(id: number): Promise<CourseDto> {
    const response = await apiClient.get<CourseDto>(`${this.BASE_PATH}/${id}`);
    return response.data!;
  }

  async createCourse(data: CreateCourseRequest): Promise<CourseDto> {
    const response = await apiClient.post<CourseDto, CreateCourseRequest>(this.BASE_PATH, data);
    return response.data!;
  }

  async updateCourse(id: number, data: UpdateCourseRequest): Promise<CourseDto> {
    const response = await apiClient.put<CourseDto, UpdateCourseRequest>(
      `${this.BASE_PATH}/${id}`,
      data
    );
    return response.data!;
  }

  async deleteCourse(id: number): Promise<void> {
    await apiClient.delete(`${this.BASE_PATH}/${id}`);
  }

  async getCourseStudents(courseId: number): Promise<StudentDto[]> {
    const response = await apiClient.get<StudentDto[]>(`${this.BASE_PATH}/${courseId}/students`);
    return response.data!;
  }

  async getCourseAssignments(courseId: number): Promise<AssignmentDto[]> {
    const response = await apiClient.get<AssignmentDto[]>(
      `${this.BASE_PATH}/${courseId}/assignments`
    );
    return response.data!;
  }

  async assignTeacher(courseId: number, teacherId: number): Promise<CourseDto> {
    const response = await apiClient.post<CourseDto>(
      `${this.BASE_PATH}/${courseId}/assign-teacher`,
      { teacherId }
    );
    return response.data!;
  }

  async exportCourses(params?: CourseQueryParams): Promise<Blob> {
    const response = await apiClient.get(`${this.BASE_PATH}/export`, {
      params,
      responseType: 'blob',
    });
    return response.data as unknown as Blob;
  }
}

export const courseService = new CourseService();
export default courseService;
