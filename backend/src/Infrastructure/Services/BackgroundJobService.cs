using Microsoft.Extensions.Logging;
using Quartz;
using NationalClothingStore.Application.Services;
using NationalClothingStore.Application.Interfaces;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Quartz.Impl.Matchers;

namespace NationalClothingStore.Infrastructure.Services;

/// <summary>
/// Background job service implementation using Quartz.NET
/// </summary>
public class BackgroundJobService : IBackgroundJobService
{
    private readonly IScheduler _scheduler;
    private readonly ILogger<BackgroundJobService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<string, JobInfo> _jobRegistry;

    public BackgroundJobService(
        IScheduler scheduler,
        ILogger<BackgroundJobService> logger,
        IServiceProvider serviceProvider)
    {
        _scheduler = scheduler;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _jobRegistry = new ConcurrentDictionary<string, JobInfo>();
    }

    public async Task ScheduleJobAsync<T>(string jobName, DateTime runAt, Dictionary<string, object>? jobData = null, CancellationToken cancellationToken = default) where T : class, IBackgroundJob
    {
        try
        {
            var jobDetail = CreateJobDetail<T>(jobName, jobData);
            var trigger = TriggerBuilder.Create()
                .StartAt(runAt)
                .WithIdentity($"{jobName}-trigger")
                .Build();

            await _scheduler.ScheduleJob(jobDetail, trigger, cancellationToken);

            RegisterJob(jobName, jobDetail, trigger, jobData);
            _logger.LogInformation("Scheduled job {JobName} to run at {RunAt}", jobName, runAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to schedule job {JobName}", jobName);
            throw;
        }
    }

    public async Task ScheduleRecurringJobAsync<T>(string jobName, string cronExpression, Dictionary<string, object>? jobData = null, CancellationToken cancellationToken = default) where T : class, IBackgroundJob
    {
        try
        {
            var jobDetail = CreateJobDetail<T>(jobName, jobData);
            var trigger = TriggerBuilder.Create()
                .WithCronSchedule(cronExpression)
                .WithIdentity($"{jobName}-trigger")
                .Build();

            await _scheduler.ScheduleJob(jobDetail, trigger, cancellationToken);

            RegisterJob(jobName, jobDetail, trigger, jobData, cronExpression);
            _logger.LogInformation("Scheduled recurring job {JobName} with cron expression {CronExpression}", jobName, cronExpression);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to schedule recurring job {JobName}", jobName);
            throw;
        }
    }

    public async Task ScheduleDelayedJobAsync<T>(string jobName, TimeSpan delay, Dictionary<string, object>? jobData = null, CancellationToken cancellationToken = default) where T : class, IBackgroundJob
    {
        try
        {
            var jobDetail = CreateJobDetail<T>(jobName, jobData);
            var trigger = TriggerBuilder.Create()
                .StartAt(DateTime.UtcNow.Add(delay))
                .WithIdentity($"{jobName}-trigger")
                .Build();

            await _scheduler.ScheduleJob(jobDetail, trigger, cancellationToken);

            RegisterJob(jobName, jobDetail, trigger, jobData);
            _logger.LogInformation("Scheduled delayed job {JobName} with delay {Delay}", jobName, delay);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to schedule delayed job {JobName}", jobName);
            throw;
        }
    }

    public async Task TriggerJobAsync<T>(string jobName, Dictionary<string, object>? jobData = null, CancellationToken cancellationToken = default) where T : class, IBackgroundJob
    {
        try
        {
            var jobDetail = CreateJobDetail<T>(jobName, jobData);
            var trigger = TriggerBuilder.Create()
                .StartNow()
                .WithIdentity($"{jobName}-trigger-{Guid.NewGuid()}")
                .Build();

            await _scheduler.ScheduleJob(jobDetail, trigger, cancellationToken);

            _logger.LogInformation("Triggered job {JobName} immediately", jobName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to trigger job {JobName}", jobName);
            throw;
        }
    }

    public async Task PauseJobAsync(string jobName, CancellationToken cancellationToken = default)
    {
        try
        {
            await _scheduler.PauseJob(new JobKey(jobName), cancellationToken);
            
            if (_jobRegistry.TryGetValue(jobName, out var jobInfo))
            {
                jobInfo.IsPaused = true;
            }

            _logger.LogInformation("Paused job {JobName}", jobName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to pause job {JobName}", jobName);
            throw;
        }
    }

    public async Task ResumeJobAsync(string jobName, CancellationToken cancellationToken = default)
    {
        try
        {
            await _scheduler.ResumeJob(new JobKey(jobName), cancellationToken);
            
            if (_jobRegistry.TryGetValue(jobName, out var jobInfo))
            {
                jobInfo.IsPaused = false;
            }

            _logger.LogInformation("Resumed job {JobName}", jobName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resume job {JobName}", jobName);
            throw;
        }
    }

    public async Task DeleteJobAsync(string jobName, CancellationToken cancellationToken = default)
    {
        try
        {
            await _scheduler.DeleteJob(new JobKey(jobName), cancellationToken);
            _jobRegistry.TryRemove(jobName, out _);

            _logger.LogInformation("Deleted job {JobName}", jobName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete job {JobName}", jobName);
            throw;
        }
    }

    public async Task<IEnumerable<JobInfo>> GetScheduledJobsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var jobKeys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup(), cancellationToken);
            var jobs = new List<JobInfo>();

            foreach (var jobKey in jobKeys)
            {
                var jobDetail = await _scheduler.GetJobDetail(jobKey, cancellationToken);
                var triggers = await _scheduler.GetTriggersOfJob(jobKey, cancellationToken);
                var currentlyExecuting = await _scheduler.GetCurrentlyExecutingJobs(cancellationToken);
                var isRunning = currentlyExecuting.Any(j => j.JobDetail.Key.Equals(jobKey));

                var jobInfo = _jobRegistry.GetValueOrDefault(jobKey.Name, new JobInfo
                {
                    Name = jobKey.Name,
                    JobType = jobDetail.JobType.Name,
                    CreatedAt = DateTime.UtcNow
                });

                jobInfo.IsRunning = isRunning;
                jobInfo.NextFireTime = triggers.FirstOrDefault()?.GetNextFireTimeUtc()?.LocalDateTime;
                jobInfo.PreviousFireTime = triggers.FirstOrDefault()?.GetPreviousFireTimeUtc()?.LocalDateTime;

                jobs.Add(jobInfo);
            }

            return jobs;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get scheduled jobs");
            throw;
        }
    }

    public async Task<IEnumerable<JobExecutionHistory>> GetJobHistoryAsync(string jobName, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        // In a real implementation, this would query a job execution history table
        // For now, return empty list as this is a placeholder
        _logger.LogInformation("Retrieved job history for {JobName}, page {Page}, size {PageSize}", jobName, page, pageSize);
        return Enumerable.Empty<JobExecutionHistory>();
    }

    public async Task<JobStatistics> GetJobStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var jobKeys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup(), cancellationToken);
            var currentlyExecuting = await _scheduler.GetCurrentlyExecutingJobs(cancellationToken);

            var statistics = new JobStatistics
            {
                TotalJobs = jobKeys.Count,
                RunningJobs = currentlyExecuting.Count,
                PausedJobs = _jobRegistry.Values.Count(j => j.IsPaused),
                CompletedExecutions = 0, // Would be retrieved from history table
                FailedExecutions = 0, // Would be retrieved from history table
                SuccessRate = 0,
                AverageExecutionTime = TimeSpan.Zero
            };

            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get job statistics");
            throw;
        }
    }

    public async Task<bool> IsJobRunningAsync(string jobName, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentlyExecuting = await _scheduler.GetCurrentlyExecutingJobs(cancellationToken);
            return currentlyExecuting.Any(j => j.JobDetail.Key.Name.Equals(jobName));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check if job {JobName} is running", jobName);
            return false;
        }
    }

    public async Task<DateTime?> GetNextRunTimeAsync(string jobName, CancellationToken cancellationToken = default)
    {
        try
        {
            var triggers = await _scheduler.GetTriggersOfJob(new JobKey(jobName), cancellationToken);
            return triggers.FirstOrDefault()?.GetNextFireTimeUtc()?.LocalDateTime;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get next run time for job {JobName}", jobName);
            return null;
        }
    }

    private IJobDetail CreateJobDetail<T>(string jobName, Dictionary<string, object>? jobData) where T : class, IBackgroundJob
    {
        var jobDataMap = new JobDataMap();
        
        if (jobData != null)
        {
            foreach (var kvp in jobData)
            {
                jobDataMap.Put(kvp.Key, kvp.Value);
            }
        }

        return JobBuilder.Create<QuartzJobWrapper<T>>()
            .WithIdentity(jobName)
            .SetJobData(jobDataMap)
            .StoreDurably()
            .Build();
    }

    private void RegisterJob(string jobName, IJobDetail jobDetail, ITrigger trigger, Dictionary<string, object>? jobData, string? cronExpression = null)
    {
        var jobInfo = new JobInfo
        {
            Name = jobName,
            JobType = jobDetail.JobType.Name,
            CronExpression = cronExpression ?? string.Empty,
            NextFireTime = trigger.GetNextFireTimeUtc()?.LocalDateTime,
            CreatedAt = DateTime.UtcNow,
            JobData = jobData ?? new Dictionary<string, object>(),
            IsPaused = false
        };

        _jobRegistry.AddOrUpdate(jobName, jobInfo, (key, oldValue) => jobInfo);
    }
}

/// <summary>
/// Quartz.NET job wrapper for background jobs
/// </summary>
public class QuartzJobWrapper<T> : IJob where T : class, IBackgroundJob
{
    private readonly ILogger<QuartzJobWrapper<T>> _logger;
    private readonly IServiceProvider _serviceProvider;

    public QuartzJobWrapper(ILogger<QuartzJobWrapper<T>> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var job = _serviceProvider.GetService<T>();
            if (job == null)
            {
                _logger.LogError("Could not resolve job service of type {JobType}", typeof(T).Name);
                return;
            }

            var jobData = new Dictionary<string, object>();
            foreach (var key in context.JobDetail.JobDataMap.Keys)
            {
                jobData[key] = context.JobDetail.JobDataMap[key]!;
            }

            var jobExecutionContext = new JobExecutionContext
            {
                JobName = context.JobDetail.Key.Name,
                JobId = context.FireInstanceId,
                FireTime = context.FireTimeUtc.LocalDateTime,
                JobData = jobData,
                RefireCount = context.RefireCount,
                CancellationToken = context.CancellationToken
            };

            _logger.LogInformation("Executing job {JobName} with ID {JobId}", jobExecutionContext.JobName, jobExecutionContext.JobId);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            await job.ExecuteAsync(jobExecutionContext, context.CancellationToken);
            stopwatch.Stop();

            _logger.LogInformation("Completed job {JobName} in {Duration}ms", jobExecutionContext.JobName, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute job {JobName}", context.JobDetail.Key.Name);
            throw new JobExecutionException($"Job {context.JobDetail.Key.Name} failed", ex);
        }
    }
}
