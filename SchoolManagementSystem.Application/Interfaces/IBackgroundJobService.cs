namespace SchoolManagementSystem.Application.Interfaces;

public interface IBackgroundJobService
{
    // Scheduled Jobs
    string ScheduleDailyAttendanceReport();
    string ScheduleWeeklyGradeSummary();
    string ScheduleMonthlyPerformanceReports();
    string ScheduleAssignmentDueReminders();

    // One-Time Jobs
    string EnqueueGPARecalculationJob(int studentId);
    string EnqueueBulkEmailJob(List<string> recipients, string subject, string body);
    string EnqueueTranscriptGenerationJob(int studentId);
    string EnqueueReportGenerationJob(string reportType, int entityId);

    // Recurring Jobs
    void ScheduleAttendanceWarningCheck();
    void ScheduleEnrollmentReminders();
    void ScheduleDataCleanupJob();

    // Job Management
    bool DeleteJob(string jobId);
    bool TriggerJob(string jobId);
}
