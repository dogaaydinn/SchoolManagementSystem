# School Management System - Frontend

Modern, responsive React + TypeScript frontend for the School Management System built with Vite, TailwindCSS, and enterprise-grade tooling.

## 🚀 Tech Stack

- **Framework**: React 18.2
- **Language**: TypeScript 5.2
- **Build Tool**: Vite 5.0
- **Styling**: TailwindCSS 3.3
- **State Management**: Zustand 4.4
- **Routing**: React Router DOM 6.20
- **Forms**: React Hook Form + Zod validation
- **API Client**: Axios 1.6
- **Data Fetching**: React Query 3.39
- **Charts**: Recharts 2.10
- **Icons**: Lucide React
- **Notifications**: Sonner

### Testing Stack

- **Unit Testing**: Vitest + React Testing Library
- **E2E Testing**: Playwright
- **Coverage**: Vitest Coverage (v8)

## 📦 Project Structure

```
client/
├── src/
│   ├── components/         # Reusable UI components
│   │   └── layouts/        # Layout components (Main, Auth)
│   ├── pages/              # Page components
│   │   ├── auth/           # Authentication pages
│   │   ├── students/       # Student management pages
│   │   ├── courses/        # Course management pages
│   │   └── grades/         # Grade management pages
│   ├── services/           # API services
│   │   ├── api.ts          # Base API client
│   │   ├── auth.service.ts # Authentication service
│   │   ├── student.service.ts
│   │   ├── course.service.ts
│   │   └── grade.service.ts
│   ├── store/              # Zustand state management
│   │   └── auth.store.ts   # Authentication store
│   ├── types/              # TypeScript type definitions
│   │   └── api.ts          # API DTOs and interfaces
│   ├── hooks/              # Custom React hooks
│   ├── utils/              # Utility functions
│   ├── assets/             # Static assets
│   ├── App.tsx             # Main app component
│   ├── main.tsx            # App entry point
│   └── index.css           # Global styles
├── public/                 # Public static files
├── index.html              # HTML template
├── vite.config.ts          # Vite configuration
├── tsconfig.json           # TypeScript configuration
├── tailwind.config.js      # TailwindCSS configuration
└── package.json            # Dependencies
```

## 🛠️ Getting Started

### Prerequisites

- Node.js 18+ and npm/yarn
- Backend API running on `http://localhost:5000` (or configure `VITE_API_URL`)

### Installation

```bash
# Install dependencies
npm install

# Copy environment variables
cp .env.example .env

# Start development server
npm run dev
```

The application will be available at `http://localhost:3000`

### Environment Variables

Create a `.env` file based on `.env.example`:

```env
VITE_API_URL=http://localhost:5000/api/v1
VITE_APP_NAME=School Management System
VITE_APP_VERSION=1.0.0
```

## 📜 Available Scripts

```bash
# Development
npm run dev              # Start dev server (port 3000)

# Building
npm run build            # Build for production
npm run preview          # Preview production build

# Testing
npm test                 # Run unit tests
npm run test:ui          # Run tests with UI
npm run test:coverage    # Generate coverage report
npm run e2e              # Run E2E tests
npm run e2e:ui           # Run E2E tests with UI

# Code Quality
npm run lint             # Lint code with ESLint
```

## 🎨 Features

### Authentication
- ✅ Login with email/password
- ✅ User registration with role selection
- ✅ JWT token management
- ✅ Protected routes
- ✅ Automatic token refresh
- ✅ Role-based access control

### Dashboard
- ✅ Key metrics (students, courses, teachers, GPA)
- ✅ Enrollment trend chart
- ✅ Grade distribution chart
- ✅ Recent activity feed
- ✅ Responsive design

### Student Management
- ✅ Student listing with search/filter
- ✅ Student details page
- ✅ Create/edit student forms
- ✅ View student transcript
- ✅ Enrollment management
- ✅ Export to Excel

### Course Management
- ✅ Course catalog with grid view
- ✅ Course details page
- ✅ Enrollment tracking
- ✅ Create/edit course forms
- ✅ Assign teachers
- ✅ View enrolled students

### Grade Management
- ✅ Grade listing with search
- ✅ Create/edit grades
- ✅ Letter grade calculation (12-point scale)
- ✅ Grade distribution visualization
- ✅ GPA calculation
- ✅ Export grades

### UI/UX
- ✅ Responsive design (mobile, tablet, desktop)
- ✅ Dark mode ready (Tailwind configuration)
- ✅ Accessible components (ARIA labels)
- ✅ Toast notifications
- ✅ Loading states
- ✅ Error handling
- ✅ Form validation

## 🔐 Authentication Flow

1. User submits login/register form
2. Frontend sends credentials to `/api/v1/auth/login` or `/api/v1/auth/register`
3. Backend validates and returns JWT token
4. Frontend stores token in localStorage
5. Axios interceptor adds `Authorization: Bearer {token}` to all requests
6. Protected routes check authentication state
7. On 401 response, user is redirected to login

## 🔄 State Management

### Zustand Stores

**Auth Store** (`store/auth.store.ts`):
- User information
- Authentication status
- Login/Register/Logout actions
- Role-based permissions

**Future Stores**:
- Student store (for student-specific state)
- Course store (for course-specific state)
- UI store (for global UI state, modals, etc.)

## 📡 API Integration

### API Client (`services/api.ts`)

Centralized API client with:
- Automatic token injection
- Request/response interceptors
- Error handling
- File upload/download support
- TypeScript generics for type safety

### Service Layer

Each domain has a dedicated service:
- `auth.service.ts` - Authentication operations
- `student.service.ts` - Student CRUD operations
- `course.service.ts` - Course CRUD operations
- `grade.service.ts` - Grade CRUD operations

All services use TypeScript interfaces from `types/api.ts` for type safety.

## 🧪 Testing

### Unit Tests

Located in `src/**/*.test.tsx`:

```bash
# Run tests
npm test

# Run with UI
npm run test:ui

# Generate coverage
npm run test:coverage
```

**Coverage Goals**:
- Overall: 80%+
- Components: 85%+
- Services: 90%+
- Utils: 95%+

### E2E Tests

Located in `e2e/`:

```bash
# Run E2E tests
npm run e2e

# Run with UI
npm run e2e:ui
```

**Test Scenarios**:
- Authentication flows
- Student management CRUD
- Course enrollment
- Grade submission
- Navigation and routing

## 🎯 TypeScript Types

All API DTOs and types are defined in `src/types/api.ts`:

- Request DTOs (Create/Update)
- Response DTOs (Entity data)
- Query parameters
- Enums (UserRole, EnrollmentStatus, etc.)
- Generic API responses (`ApiResponse<T>`, `PaginatedResponse<T>`)

## 🚀 Deployment

### Build for Production

```bash
npm run build
```

Builds to `dist/` directory with:
- Minified JavaScript bundles
- Code splitting (vendor, chart, table chunks)
- Source maps
- Optimized assets

### Preview Production Build

```bash
npm run preview
```

### Deploy to Hosting

**Netlify/Vercel**:
```bash
# Build command
npm run build

# Publish directory
dist
```

**Docker**:
```dockerfile
FROM node:18-alpine as build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

## 🔧 Configuration

### Vite Config (`vite.config.ts`)

- React plugin
- Path aliases (`@/` → `src/`)
- API proxy (`/api` → `http://localhost:5000`)
- Code splitting optimization
- Vitest configuration

### Tailwind Config (`tailwind.config.js`)

- Custom color palette (primary shades)
- Font family (Inter)
- Component classes
- Utility extensions

### TypeScript Config

- Strict mode enabled
- Path mapping (`@/*`)
- ES2020 target
- React JSX transform

## 📊 Performance Optimizations

- ✅ Code splitting (React.lazy, dynamic imports)
- ✅ Vendor chunk separation
- ✅ Tree shaking
- ✅ Image optimization
- ✅ React Query caching (5min stale time)
- ✅ Memoization for expensive calculations
- ✅ Virtualization for long lists (future)

## 🔮 Future Enhancements

- [ ] Add more comprehensive E2E tests
- [ ] Implement WebSocket for real-time updates
- [ ] Add file upload for student photos
- [ ] Implement bulk operations (import CSV)
- [ ] Add advanced filters and sorting
- [ ] Implement calendar view for schedules
- [ ] Add notifications center
- [ ] Implement user preferences/settings
- [ ] Add multi-language support (i18n)
- [ ] Implement progressive web app (PWA)
- [ ] Add dark mode toggle
- [ ] Implement print-friendly views

## 🤝 Contributing

1. Follow the existing code structure
2. Use TypeScript for all new files
3. Follow the component naming convention
4. Add unit tests for new features
5. Ensure all tests pass before committing
6. Use meaningful commit messages

## 📝 Code Style

- **Components**: PascalCase (`StudentCard.tsx`)
- **Utilities**: camelCase (`formatDate.ts`)
- **Constants**: UPPER_SNAKE_CASE
- **Props Interfaces**: `ComponentNameProps`
- **Indentation**: 2 spaces
- **Quotes**: Single quotes
- **Semicolons**: Required

## 📄 License

This project is part of the School Management System and follows the same license as the parent repository.

## 🆘 Support

For issues, questions, or contributions, please refer to the main repository README or contact the development team.

---

**Version**: 1.0.0
**Last Updated**: 2025-11-13
**Phase**: Phase 6 - Frontend Integration
