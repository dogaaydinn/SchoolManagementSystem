# Multi-stage Docker build for School Management System API

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy project files
COPY ["SchoolManagementSystem.API/SchoolManagementSystem.API.csproj", "SchoolManagementSystem.API/"]
COPY ["SchoolManagementSystem.Core/SchoolManagementSystem.Core.csproj", "SchoolManagementSystem.Core/"]
COPY ["SchoolManagementSystem.Infrastructure/SchoolManagementSystem.Infrastructure.csproj", "SchoolManagementSystem.Infrastructure/"]
COPY ["SchoolManagementSystem.Application/SchoolManagementSystem.Application.csproj", "SchoolManagementSystem.Application/"]

# Restore dependencies
RUN dotnet restore "SchoolManagementSystem.API/SchoolManagementSystem.API.csproj"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/SchoolManagementSystem.API"
RUN dotnet build "SchoolManagementSystem.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "SchoolManagementSystem.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published files from publish stage
COPY --from=publish /app/publish .

# Create logs directory and set permissions
RUN mkdir -p /app/logs && chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose ports
EXPOSE 80
EXPOSE 443

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD curl -f http://localhost/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "SchoolManagementSystem.API.dll"]
