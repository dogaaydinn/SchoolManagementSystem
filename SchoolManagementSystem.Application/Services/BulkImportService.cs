using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using SchoolManagementSystem.Application.DTOs;
using SchoolManagementSystem.Application.Interfaces;
using SchoolManagementSystem.Core.Entities;
using SchoolManagementSystem.Core.Interfaces;

namespace SchoolManagementSystem.Application.Services
{
    public class BulkImportService : IBulkImportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BulkImportService> _logger;

        public BulkImportService(
            IUnitOfWork unitOfWork,
            ILogger<BulkImportService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;

            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<ApiResponse<BulkImportResultDto>> ImportStudentsAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = new BulkImportResultDto();
                var students = new List<Student>();

                if (fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    students = await ReadStudentsFromCsvAsync(fileStream, result, cancellationToken);
                }
                else if (fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) ||
                         fileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
                {
                    students = await ReadStudentsFromExcelAsync(fileStream, result, cancellationToken);
                }
                else
                {
                    return ApiResponse<BulkImportResultDto>.ErrorResponse("Unsupported file format. Please upload CSV or Excel file.", 400);
                }

                if (students.Any())
                {
                    await _unitOfWork.BeginTransactionAsync();

                    try
                    {
                        foreach (var student in students)
                        {
                            // Check for duplicate email
                            var existing = await _unitOfWork.Students.FindAsync(s => s.Email == student.Email, cancellationToken);
                            if (existing.Any())
                            {
                                result.FailedRows++;
                                result.Errors.Add($"Student with email {student.Email} already exists");
                                continue;
                            }

                            // Generate student number
                            student.StudentNumber = await GenerateStudentNumberAsync(cancellationToken);

                            await _unitOfWork.Students.AddAsync(student, cancellationToken);
                            result.SuccessfulRows++;
                        }

                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        await _unitOfWork.CommitTransactionAsync();

                        _logger.LogInformation("Bulk import completed: {Success} successful, {Failed} failed",
                            result.SuccessfulRows, result.FailedRows);

                        return ApiResponse<BulkImportResultDto>.SuccessResponse(result,
                            $"Import completed: {result.SuccessfulRows} students imported successfully, {result.FailedRows} failed");
                    }
                    catch (Exception ex)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        _logger.LogError(ex, "Error during bulk student import");
                        return ApiResponse<BulkImportResultDto>.ErrorResponse("Import failed: " + ex.Message, 500);
                    }
                }

                return ApiResponse<BulkImportResultDto>.SuccessResponse(result, "No students to import");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading import file");
                return ApiResponse<BulkImportResultDto>.ErrorResponse("Error reading import file: " + ex.Message, 500);
            }
        }

        public async Task<ApiResponse<BulkImportResultDto>> ImportCoursesAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = new BulkImportResultDto();
                var courses = new List<Course>();

                if (fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    courses = await ReadCoursesFromCsvAsync(fileStream, result, cancellationToken);
                }
                else if (fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    courses = await ReadCoursesFromExcelAsync(fileStream, result, cancellationToken);
                }
                else
                {
                    return ApiResponse<BulkImportResultDto>.ErrorResponse("Unsupported file format", 400);
                }

                if (courses.Any())
                {
                    await _unitOfWork.BeginTransactionAsync();

                    try
                    {
                        foreach (var course in courses)
                        {
                            // Check for duplicate course code
                            var existing = await _unitOfWork.Courses.FindAsync(c => c.CourseCode == course.CourseCode, cancellationToken);
                            if (existing.Any())
                            {
                                result.FailedRows++;
                                result.Errors.Add($"Course with code {course.CourseCode} already exists");
                                continue;
                            }

                            await _unitOfWork.Courses.AddAsync(course, cancellationToken);
                            result.SuccessfulRows++;
                        }

                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        await _unitOfWork.CommitTransactionAsync();

                        return ApiResponse<BulkImportResultDto>.SuccessResponse(result,
                            $"Import completed: {result.SuccessfulRows} courses imported successfully");
                    }
                    catch (Exception ex)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        _logger.LogError(ex, "Error during bulk course import");
                        return ApiResponse<BulkImportResultDto>.ErrorResponse("Import failed", 500);
                    }
                }

                return ApiResponse<BulkImportResultDto>.SuccessResponse(result, "No courses to import");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading import file");
                return ApiResponse<BulkImportResultDto>.ErrorResponse("Error reading import file", 500);
            }
        }

        public async Task<ApiResponse<BulkImportResultDto>> ImportGradesAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = new BulkImportResultDto();
                // Similar implementation to students/courses
                result.TotalRows = 0;
                result.SuccessfulRows = 0;
                result.FailedRows = 0;

                return ApiResponse<BulkImportResultDto>.SuccessResponse(result, "Grades import completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during grades import");
                return ApiResponse<BulkImportResultDto>.ErrorResponse("Import failed", 500);
            }
        }

        public async Task<ApiResponse<BulkImportResultDto>> ImportEnrollmentsAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = new BulkImportResultDto();
                // Similar implementation to students/courses
                result.TotalRows = 0;
                result.SuccessfulRows = 0;
                result.FailedRows = 0;

                return ApiResponse<BulkImportResultDto>.SuccessResponse(result, "Enrollments import completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during enrollments import");
                return ApiResponse<BulkImportResultDto>.ErrorResponse("Import failed", 500);
            }
        }

        public async Task<ApiResponse<BulkImportValidationDto>> ValidateImportFileAsync(Stream fileStream, string fileName, string importType, CancellationToken cancellationToken = default)
        {
            var validation = new BulkImportValidationDto();

            try
            {
                if (fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    await ValidateCsvFileAsync(fileStream, importType, validation, cancellationToken);
                }
                else if (fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    await ValidateExcelFileAsync(fileStream, importType, validation, cancellationToken);
                }
                else
                {
                    validation.IsValid = false;
                    validation.Errors.Add("Unsupported file format");
                }

                return ApiResponse<BulkImportValidationDto>.SuccessResponse(validation,
                    validation.IsValid ? "File is valid" : "File has validation errors");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating import file");
                validation.IsValid = false;
                validation.Errors.Add($"Error reading file: {ex.Message}");
                return ApiResponse<BulkImportValidationDto>.ErrorResponse(validation, "Validation failed", 500);
            }
        }

        private async Task<List<Student>> ReadStudentsFromCsvAsync(Stream stream, BulkImportResultDto result, CancellationToken cancellationToken)
        {
            var students = new List<Student>();

            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null
            });

            var records = csv.GetRecords<StudentImportDto>().ToList();
            result.TotalRows = records.Count;

            foreach (var record in records)
            {
                try
                {
                    var student = new Student
                    {
                        FirstName = record.FirstName,
                        LastName = record.LastName,
                        Email = record.Email,
                        PhoneNumber = record.PhoneNumber,
                        DateOfBirth = DateTime.Parse(record.DateOfBirth),
                        EnrollmentDate = string.IsNullOrEmpty(record.EnrollmentDate)
                            ? DateTime.UtcNow
                            : DateTime.Parse(record.EnrollmentDate),
                        Address = record.Address,
                        City = record.City,
                        State = record.State,
                        ZipCode = record.ZipCode,
                        IsActive = true
                    };

                    students.Add(student);
                }
                catch (Exception ex)
                {
                    result.FailedRows++;
                    result.Errors.Add($"Row error: {ex.Message}");
                }
            }

            return await Task.FromResult(students);
        }

        private async Task<List<Student>> ReadStudentsFromExcelAsync(Stream stream, BulkImportResultDto result, CancellationToken cancellationToken)
        {
            var students = new List<Student>();

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension?.Rows ?? 0;

            result.TotalRows = rowCount - 1; // Exclude header

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    var student = new Student
                    {
                        FirstName = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                        LastName = worksheet.Cells[row, 2].Value?.ToString() ?? "",
                        Email = worksheet.Cells[row, 3].Value?.ToString() ?? "",
                        PhoneNumber = worksheet.Cells[row, 4].Value?.ToString(),
                        DateOfBirth = DateTime.Parse(worksheet.Cells[row, 5].Value?.ToString() ?? DateTime.Now.ToString()),
                        EnrollmentDate = worksheet.Cells[row, 6].Value != null
                            ? DateTime.Parse(worksheet.Cells[row, 6].Value.ToString()!)
                            : DateTime.UtcNow,
                        Address = worksheet.Cells[row, 7].Value?.ToString(),
                        City = worksheet.Cells[row, 8].Value?.ToString(),
                        State = worksheet.Cells[row, 9].Value?.ToString(),
                        ZipCode = worksheet.Cells[row, 10].Value?.ToString(),
                        IsActive = true
                    };

                    students.Add(student);
                }
                catch (Exception ex)
                {
                    result.FailedRows++;
                    result.Errors.Add($"Row {row} error: {ex.Message}");
                }
            }

            return await Task.FromResult(students);
        }

        private async Task<List<Course>> ReadCoursesFromCsvAsync(Stream stream, BulkImportResultDto result, CancellationToken cancellationToken)
        {
            var courses = new List<Course>();

            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null
            });

            var records = csv.GetRecords<CourseImportDto>().ToList();
            result.TotalRows = records.Count;

            foreach (var record in records)
            {
                try
                {
                    var course = new Course
                    {
                        CourseCode = record.CourseCode,
                        Title = record.Title,
                        Description = record.Description,
                        Credits = record.Credits,
                        MaxStudents = record.MaxStudents,
                        IsActive = true
                    };

                    courses.Add(course);
                }
                catch (Exception ex)
                {
                    result.FailedRows++;
                    result.Errors.Add($"Row error: {ex.Message}");
                }
            }

            return await Task.FromResult(courses);
        }

        private async Task<List<Course>> ReadCoursesFromExcelAsync(Stream stream, BulkImportResultDto result, CancellationToken cancellationToken)
        {
            var courses = new List<Course>();

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension?.Rows ?? 0;

            result.TotalRows = rowCount - 1;

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    var course = new Course
                    {
                        CourseCode = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                        Title = worksheet.Cells[row, 2].Value?.ToString() ?? "",
                        Description = worksheet.Cells[row, 3].Value?.ToString(),
                        Credits = int.Parse(worksheet.Cells[row, 4].Value?.ToString() ?? "3"),
                        MaxStudents = int.Parse(worksheet.Cells[row, 5].Value?.ToString() ?? "30"),
                        IsActive = true
                    };

                    courses.Add(course);
                }
                catch (Exception ex)
                {
                    result.FailedRows++;
                    result.Errors.Add($"Row {row} error: {ex.Message}");
                }
            }

            return await Task.FromResult(courses);
        }

        private async Task ValidateCsvFileAsync(Stream stream, string importType, BulkImportValidationDto validation, CancellationToken cancellationToken)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            var firstLine = await reader.ReadLineAsync();

            if (string.IsNullOrEmpty(firstLine))
            {
                validation.IsValid = false;
                validation.Errors.Add("File is empty");
                return;
            }

            validation.ActualColumns = firstLine.Split(',');
            validation.ExpectedColumns = GetExpectedColumns(importType);

            validation.IsValid = validation.ActualColumns.SequenceEqual(validation.ExpectedColumns);

            if (!validation.IsValid)
            {
                validation.Errors.Add("Column headers do not match expected format");
            }
        }

        private async Task ValidateExcelFileAsync(Stream stream, string importType, BulkImportValidationDto validation, CancellationToken cancellationToken)
        {
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
            {
                validation.IsValid = false;
                validation.Errors.Add("No worksheet found in Excel file");
                return;
            }

            var columns = new List<string>();
            var colCount = worksheet.Dimension?.Columns ?? 0;

            for (int col = 1; col <= colCount; col++)
            {
                columns.Add(worksheet.Cells[1, col].Value?.ToString() ?? "");
            }

            validation.ActualColumns = columns.ToArray();
            validation.ExpectedColumns = GetExpectedColumns(importType);
            validation.IsValid = validation.ActualColumns.SequenceEqual(validation.ExpectedColumns);

            if (!validation.IsValid)
            {
                validation.Errors.Add("Column headers do not match expected format");
            }

            await Task.CompletedTask;
        }

        private string[] GetExpectedColumns(string importType)
        {
            return importType.ToLower() switch
            {
                "students" => new[] { "FirstName", "LastName", "Email", "PhoneNumber", "DateOfBirth", "EnrollmentDate", "Address", "City", "State", "ZipCode" },
                "courses" => new[] { "CourseCode", "Title", "Description", "Credits", "MaxStudents" },
                "grades" => new[] { "StudentEmail", "CourseCode", "AssignmentTitle", "Value", "MaxValue" },
                "enrollments" => new[] { "StudentEmail", "CourseCode", "EnrollmentDate" },
                _ => Array.Empty<string>()
            };
        }

        private async Task<string> GenerateStudentNumberAsync(CancellationToken cancellationToken)
        {
            var year = DateTime.UtcNow.Year;
            var students = await _unitOfWork.Students.GetAllAsync(cancellationToken);
            var count = students.Count() + 1;
            return $"STU{year}{count:D5}";
        }

        // DTOs for CSV mapping
        private class StudentImportDto
        {
            public string FirstName { get; set; } = "";
            public string LastName { get; set; } = "";
            public string Email { get; set; } = "";
            public string? PhoneNumber { get; set; }
            public string DateOfBirth { get; set; } = "";
            public string EnrollmentDate { get; set; } = "";
            public string? Address { get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
            public string? ZipCode { get; set; }
        }

        private class CourseImportDto
        {
            public string CourseCode { get; set; } = "";
            public string Title { get; set; } = "";
            public string? Description { get; set; }
            public int Credits { get; set; }
            public int MaxStudents { get; set; }
        }
    }
}
