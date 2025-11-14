import { create } from 'zustand';
import { devtools, persist } from 'zustand/middleware';
import { UserDto, LoginRequest, RegisterRequest } from '@/types/api';
import { authService } from '@/services/auth.service';

interface AuthState {
  user: UserDto | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;

  // Actions
  login: (credentials: LoginRequest) => Promise<void>;
  register: (data: RegisterRequest) => Promise<void>;
  logout: () => Promise<void>;
  getCurrentUser: () => Promise<void>;
  clearError: () => void;
  hasRole: (role: string) => boolean;
  hasAnyRole: (roles: string[]) => boolean;
}

export const useAuthStore = create<AuthState>()(
  devtools(
    persist(
      (set, get) => ({
        user: authService.getUser(),
        token: authService.getToken(),
        isAuthenticated: authService.isAuthenticated(),
        isLoading: false,
        error: null,

        login: async (credentials: LoginRequest) => {
          set({ isLoading: true, error: null });
          try {
            const authResponse = await authService.login(credentials);
            set({
              user: authResponse.user,
              token: authResponse.token,
              isAuthenticated: true,
              isLoading: false,
            });
          } catch (error: any) {
            const errorMessage = error.response?.data?.message || 'Login failed';
            set({
              error: errorMessage,
              isLoading: false,
              isAuthenticated: false,
            });
            throw error;
          }
        },

        register: async (data: RegisterRequest) => {
          set({ isLoading: true, error: null });
          try {
            const authResponse = await authService.register(data);
            set({
              user: authResponse.user,
              token: authResponse.token,
              isAuthenticated: true,
              isLoading: false,
            });
          } catch (error: any) {
            const errorMessage = error.response?.data?.message || 'Registration failed';
            set({
              error: errorMessage,
              isLoading: false,
              isAuthenticated: false,
            });
            throw error;
          }
        },

        logout: async () => {
          set({ isLoading: true });
          try {
            await authService.logout();
          } finally {
            set({
              user: null,
              token: null,
              isAuthenticated: false,
              isLoading: false,
              error: null,
            });
          }
        },

        getCurrentUser: async () => {
          set({ isLoading: true, error: null });
          try {
            const user = await authService.getCurrentUser();
            set({
              user,
              isAuthenticated: true,
              isLoading: false,
            });
          } catch (error: any) {
            const errorMessage = error.response?.data?.message || 'Failed to fetch user';
            set({
              error: errorMessage,
              isLoading: false,
              user: null,
              token: null,
              isAuthenticated: false,
            });
            throw error;
          }
        },

        clearError: () => set({ error: null }),

        hasRole: (role: string) => {
          const { user } = get();
          return user?.role === role;
        },

        hasAnyRole: (roles: string[]) => {
          const { user } = get();
          return user ? roles.includes(user.role) : false;
        },
      }),
      {
        name: 'auth-storage',
        partialize: (state) => ({
          user: state.user,
          token: state.token,
          isAuthenticated: state.isAuthenticated,
        }),
      }
    ),
    { name: 'AuthStore' }
  )
);
