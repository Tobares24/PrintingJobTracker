using Microsoft.Extensions.Logging;
using PrintingJobTracker.Application.DTOs;
using PrintingJobTracker.Application.Interfaces;
using PrintingJobTracker.Application.Specifications;
using PrintingJobTracker.Domain.Entities;
using PrintingJobTracker.Domain.Entities.Enums;
using PrintingJobTracker.Infrastructure.Repository.Jobs;

namespace PrintingJobTracker.Application.Services
{
    public sealed class JobService(
        ILogger<JobService> logger,
        IJobRepository jobRepository) : IJobService
    {
        private readonly ILogger<JobService> _logger = logger;
        private readonly IJobRepository _jobRepository = jobRepository;

        public async Task AddJobAsync(string traceId, CreateJobRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                Job job = new()
                {
                    ClientId = request.ClientId,
                    JobName = request.JobName,
                    Quantity = request.Quantity,
                    Carrier = request.Carrier,
                    MailDeadline = request.MailDeadline,
                    CreatedAt = DateTime.UtcNow,
                    CurrentStatus = JobStatus.Received,
                    IsDeleted = false
                };

                _logger.LogInformation("{TraceId} - Adding job through service: {JobId}", traceId, job.Id);

                await _jobRepository.AddAsync(traceId, job, cancellationToken);

                var history = new JobStatusHistory
                {
                    JobId = job.Id,
                    Status = JobStatus.Received,
                    Note = "Job created and received",
                    ChangedAt = DateTime.UtcNow
                };

                await _jobRepository.AddHistoryAsync(traceId, history, cancellationToken);

                _logger.LogInformation("{TraceId} - Job added successfully: {JobId}", traceId, job.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error adding job in service: {Message}", traceId, ex.Message);
                throw;
            }
        }

        public async Task<JobResponse> GetJobsAsync(string traceId, JobFilterRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Fetching jobs through service", traceId);

                var specification = new BaseSpecification<Job>();

                if (!string.IsNullOrWhiteSpace(request.JobName))
                    specification.AddCriteria(j => j.JobName != null && j.JobName.StartsWith(request.JobName));

                if (request.Status is not null)
                    specification.AddCriteria(j => j.CurrentStatus == request.Status);

                specification.ApplyPaging(request.Skip, request.Take);

                var jobs = await _jobRepository.GetJobsAsync(traceId, specification, cancellationToken);
                var totalCount = await _jobRepository.GetCountAsync(traceId, specification, cancellationToken);

                List<JobModel> items = jobs.Select(job => new JobModel(
                    job.Id,
                    $"{job.Client?.FirstName ?? string.Empty} {job.Client?.LastName ?? string.Empty} {job.Client?.SecondLastName ?? string.Empty}".Trim(),
                    job.JobName ?? string.Empty,
                    job.Quantity,
                    job.Carrier.ToString(),
                    job.CurrentStatus,
                    job.CreatedAt
                )).ToList();

                return new JobResponse(items, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error fetching jobs in service: {Message}", traceId, ex.Message);
                throw;
            }
        }

        public async Task<Job?> GetByIdAsync(string traceId, int jobId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Fetching job by Id through service: {JobId}", traceId, jobId);

                return await _jobRepository.GetByIdAsync(traceId, jobId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error fetching job by Id in service: {Message}", traceId, ex.Message);
                throw;
            }
        }

        public async Task<bool> AdvanceJobStatusAsync(string traceId, int jobId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Advancing job status: {JobId}", traceId, jobId);

                var job = await _jobRepository.GetByIdAsync(traceId, jobId, cancellationToken);
                if (job is null)
                {
                    _logger.LogWarning("{TraceId} - Job not found: {JobId}", traceId, jobId);
                    return false;
                }

                if (job.CurrentStatus == JobStatus.Delivered || job.CurrentStatus == JobStatus.Exception)
                {
                    _logger.LogWarning("{TraceId} - Cannot advance job in status: {Status}", traceId, job.CurrentStatus);
                    return false;
                }

                var nextStatus = GetNextStatus(job.CurrentStatus);
                if (nextStatus == job.CurrentStatus)
                {
                    _logger.LogWarning("{TraceId} - No next status available for: {Status}", traceId, job.CurrentStatus);
                    return false;
                }

                job.CurrentStatus = nextStatus;
                await _jobRepository.UpdateAsync(traceId, job, cancellationToken);

                var history = new JobStatusHistory
                {
                    JobId = job.Id,
                    Status = nextStatus,
                    Note = $"Status advanced to {nextStatus}",
                    ChangedAt = DateTime.UtcNow
                };

                await _jobRepository.AddHistoryAsync(traceId, history, cancellationToken);

                await _jobRepository.UpdateAsync(traceId, job, cancellationToken);

                _logger.LogInformation("{TraceId} - Job status advanced successfully: {JobId} -> {Status}", traceId, jobId, nextStatus);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error advancing job status: {Message}", traceId, ex.Message);
                throw;
            }
        }

        public async Task<bool> SetJobExceptionAsync(string traceId, int jobId, string note, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Setting job exception: {JobId}", traceId, jobId);

                var job = await _jobRepository.GetByIdAsync(traceId, jobId, cancellationToken);
                if (job is null)
                {
                    _logger.LogWarning("{TraceId} - Job not found: {JobId}", traceId, jobId);
                    return false;
                }

                if (job.CurrentStatus == JobStatus.Exception)
                {
                    _logger.LogWarning("{TraceId} - Job is already in Exception status: {JobId}", traceId, jobId);
                    return false;
                }

                job.CurrentStatus = JobStatus.Exception;
                await _jobRepository.UpdateAsync(traceId, job, cancellationToken);

                var history = new JobStatusHistory
                {
                    JobId = job.Id,
                    Status = JobStatus.Exception,
                    Note = note,
                    ChangedAt = DateTime.UtcNow
                };
                await _jobRepository.AddHistoryAsync(traceId, history, cancellationToken);

                _logger.LogInformation("{TraceId} - Job marked as exception: {JobId}", traceId, jobId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error setting job exception: {Message}", traceId, ex.Message);
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetJobCountsByStatusAsync(string traceId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Fetching job counts by status", traceId);

                return await _jobRepository.GetJobCountsByStatusAsync(traceId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error fetching job counts: {Message}", traceId, ex.Message);
                throw;
            }
        }

        public async Task<List<JobStatusHistory>> GetJobHistoryAsync(string traceId, int jobId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Fetching job history: {JobId}", traceId, jobId);

                return await _jobRepository.GetJobHistoryAsync(traceId, jobId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error fetching job history: {Message}", traceId, ex.Message);
                throw;
            }
        }

        private static JobStatus GetNextStatus(JobStatus currentStatus)
        {
            return currentStatus switch
            {
                JobStatus.Received => JobStatus.Printing,
                JobStatus.Printing => JobStatus.Inserting,
                JobStatus.Inserting => JobStatus.Mailed,
                JobStatus.Mailed => JobStatus.Delivered,
                _ => currentStatus
            };
        }
    }
}