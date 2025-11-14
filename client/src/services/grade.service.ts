import apiClient from './api';
import {
  GradeDto,
  CreateGradeRequest,
  PaginatedResponse,
  GradeQueryParams,
  GradeDistribution,
} from '@/types/api';

export class GradeService {
  private readonly BASE_PATH = '/grades';

  async getGrades(params?: GradeQueryParams): Promise<PaginatedResponse<GradeDto>> {
    const response = await apiClient.get<PaginatedResponse<GradeDto>>(this.BASE_PATH, {
      params,
    });
    return response.data!;
  }

  async getGradeById(id: number): Promise<GradeDto> {
    const response = await apiClient.get<GradeDto>(`${this.BASE_PATH}/${id}`);
    return response.data!;
  }

  async createGrade(data: CreateGradeRequest): Promise<GradeDto> {
    const response = await apiClient.post<GradeDto, CreateGradeRequest>(this.BASE_PATH, data);
    return response.data!;
  }

  async updateGrade(id: number, data: Partial<CreateGradeRequest>): Promise<GradeDto> {
    const response = await apiClient.put<GradeDto, Partial<CreateGradeRequest>>(
      `${this.BASE_PATH}/${id}`,
      data
    );
    return response.data!;
  }

  async deleteGrade(id: number): Promise<void> {
    await apiClient.delete(`${this.BASE_PATH}/${id}`);
  }

  async getGradeDistribution(courseId: number): Promise<GradeDistribution[]> {
    const response = await apiClient.get<GradeDistribution[]>(
      `${this.BASE_PATH}/distribution/${courseId}`
    );
    return response.data!;
  }

  async calculateStudentGPA(studentId: number): Promise<number> {
    const response = await apiClient.get<number>(`${this.BASE_PATH}/gpa/${studentId}`);
    return response.data!;
  }

  async exportGrades(params?: GradeQueryParams): Promise<Blob> {
    const response = await apiClient.get(`${this.BASE_PATH}/export`, {
      params,
      responseType: 'blob',
    });
    return response.data as unknown as Blob;
  }
}

export const gradeService = new GradeService();
export default gradeService;
