import apiClient from './api';
import {
  LoginRequest,
  RegisterRequest,
  AuthResponse,
  UserDto,
  ApiResponse,
} from '@/types/api';

export class AuthService {
  private readonly AUTH_TOKEN_KEY = 'auth_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';
  private readonly USER_KEY = 'user';

  async login(credentials: LoginRequest): Promise<AuthResponse> {
    const response = await apiClient.post<AuthResponse, LoginRequest>('/auth/login', credentials);

    if (response.success && response.data) {
      this.setAuthData(response.data);
    }

    return response.data!;
  }

  async register(data: RegisterRequest): Promise<AuthResponse> {
    const response = await apiClient.post<AuthResponse, RegisterRequest>('/auth/register', data);

    if (response.success && response.data) {
      this.setAuthData(response.data);
    }

    return response.data!;
  }

  async getCurrentUser(): Promise<UserDto> {
    const response = await apiClient.get<UserDto>('/auth/me');

    if (response.success && response.data) {
      this.setUser(response.data);
    }

    return response.data!;
  }

  async logout(): Promise<void> {
    try {
      await apiClient.post('/auth/logout');
    } finally {
      this.clearAuthData();
    }
  }

  async refreshToken(): Promise<AuthResponse> {
    const refreshToken = this.getRefreshToken();

    if (!refreshToken) {
      throw new Error('No refresh token available');
    }

    const response = await apiClient.post<AuthResponse>('/auth/refresh', {
      refreshToken,
    });

    if (response.success && response.data) {
      this.setAuthData(response.data);
    }

    return response.data!;
  }

  // Token management
  getToken(): string | null {
    return localStorage.getItem(this.AUTH_TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  getUser(): UserDto | null {
    const userJson = localStorage.getItem(this.USER_KEY);
    return userJson ? JSON.parse(userJson) : null;
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    const user = this.getUser();
    return !!(token && user);
  }

  hasRole(role: string): boolean {
    const user = this.getUser();
    return user?.role === role;
  }

  hasAnyRole(roles: string[]): boolean {
    const user = this.getUser();
    return user ? roles.includes(user.role) : false;
  }

  private setAuthData(authResponse: AuthResponse): void {
    localStorage.setItem(this.AUTH_TOKEN_KEY, authResponse.token);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, authResponse.refreshToken);
    this.setUser(authResponse.user);
  }

  private setUser(user: UserDto): void {
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
  }

  private clearAuthData(): void {
    localStorage.removeItem(this.AUTH_TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
  }
}

export const authService = new AuthService();
export default authService;
