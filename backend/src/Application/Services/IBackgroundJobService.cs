namespace NationalClothingStore.Application.Services;

/// <summary>
/// Background job service interface
/// </summary>
public interface IBackgroundJobService
{
    /// <summary>
    /// Schedules a job to run once at a specific time
    /// </summary>
    Task ScheduleJobAsync<T>(string jobName, DateTime runAt, Dictionary<string, object>? jobData = null, CancellationToken cancellationToken = default) where T : class, IBackgroundJob;

    /// <summary>
    /// Schedules a recurring job with a cron expression
    /// </summary>
    Task ScheduleRecurringJobAsync<T>(string jobName, string cronExpression, Dictionary<string, object>? jobData = null, CancellationToken cancellationToken = default) where T : class, IBackgroundJob;

    /// <summary>
    /// Schedules a job to run after a delay
    /// </summary>
    Task ScheduleDelayedJobAsync<T>(string jobName, TimeSpan delay, Dictionary<string, object>? jobData = null, CancellationToken cancellationToken = default) where T : class, IBackgroundJob;

    /// <summary>
    /// Triggers a job immediately
    /// </summary>
    Task TriggerJobAsync<T>(string jobName, Dictionary<string, object>? jobData = null, CancellationToken cancellationToken = default) where T : class, IBackgroundJob;

    /// <summary>
    /// Pauses a scheduled job
    /// </summary>
    Task PauseJobAsync(string jobName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resumes a paused job
    /// </summary>
    Task ResumeJobAsync(string jobName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a scheduled job
    /// </summary>
    Task DeleteJobAsync(string jobName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all scheduled jobs
    /// </summary>
    Task<IEnumerable<JobInfo>> GetScheduledJobsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets job execution history
    /// </summary>
    Task<IEnumerable<JobExecutionHistory>> GetJobHistoryAsync(string jobName, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets job statistics
    /// </summary>
    Task<JobStatistics> GetJobStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a job is currently running
    /// </summary>
    Task<bool> IsJobRunningAsync(string jobName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the next scheduled run time for a job
    /// </summary>
    Task<DateTime?> GetNextRunTimeAsync(string jobName, CancellationToken cancellationToken = default);
}

/// <summary>
/// Background job interface
/// </summary>
public interface IBackgroundJob
{
    /// <summary>
    /// Executes the job
    /// </summary>
    Task ExecuteAsync(JobExecutionContext context, CancellationToken cancellationToken = default);
}

/// <summary>
/// Job execution context
/// </summary>
public class JobExecutionContext
{
    public string JobName { get; set; } = string.Empty;
    public string JobId { get; set; } = string.Empty;
    public DateTime FireTime { get; set; }
    public Dictionary<string, object> JobData { get; set; } = new();
    public int RefireCount { get; set; }
    public CancellationToken CancellationToken { get; set; }
}

/// <summary>
/// Job information model
/// </summary>
public class JobInfo
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string JobType { get; set; } = string.Empty;
    public string CronExpression { get; set; } = string.Empty;
    public DateTime? NextFireTime { get; set; }
    public DateTime? PreviousFireTime { get; set; }
    public bool IsRunning { get; set; }
    public bool IsPaused { get; set; }
    public int ExecutionCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public Dictionary<string, object> JobData { get; set; } = new();
}

/// <summary>
/// Job execution history model
/// </summary>
public class JobExecutionHistory
{
    public string JobName { get; set; } = string.Empty;
    public string JobId { get; set; } = string.Empty;
    public DateTime ExecutionTime { get; set; }
    public TimeSpan Duration { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Result { get; set; }
    public int RefireCount { get; set; }
}

/// <summary>
/// Job statistics model
/// </summary>
public class JobStatistics
{
    public int TotalJobs { get; set; }
    public int RunningJobs { get; set; }
    public int PausedJobs { get; set; }
    public int CompletedExecutions { get; set; }
    public int FailedExecutions { get; set; }
    public double SuccessRate { get; set; }
    public TimeSpan AverageExecutionTime { get; set; }
    public Dictionary<string, int> ExecutionsByJob { get; set; } = new();
    public Dictionary<DateTime, int> ExecutionsByDay { get; set; } = new();
}
