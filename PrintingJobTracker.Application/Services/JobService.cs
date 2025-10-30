using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PrintingJobTracker.Application.DTOs;
using PrintingJobTracker.Application.Hubs;
using PrintingJobTracker.Application.Interfaces;
using PrintingJobTracker.Application.Specifications;
using PrintingJobTracker.Domain.Entities;
using PrintingJobTracker.Infrastructure.Repository.Jobs;

namespace PrintingJobTracker.Application.Services
{
    public sealed class JobService(
        ILogger<JobService> logger,
        IJobRepository jobRepository, IHubContext<AppHub> hubContext) : BaseService(hubContext), IJobService
    {
        private readonly ILogger<JobService> _logger = logger;
        private readonly IJobRepository _jobRepository = jobRepository;

        public async Task<List<Job>> GetJobsAsync(string traceId, JobFilterRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Fetching jobs through service", traceId);

                var specification = new BaseSpecification<Job>();

                if (!string.IsNullOrWhiteSpace(request.JobName))
                    specification.AddCriteria(j => j.JobName != null && j.JobName.StartsWith(request.JobName));

                if (request.MailDeadlineFrom is not null)
                    specification.AddCriteria(j => j.MailDeadline >= request.MailDeadlineFrom);

                if (request.MailDeadlineTo is not null)
                    specification.AddCriteria(j => j.MailDeadline <= request.MailDeadlineTo);

                specification.ApplyPaging(request.Skip, request.Take);

                var jobs = await _jobRepository.GetJobsAsync(traceId, specification, cancellationToken);

                return jobs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error fetching jobs in service: {Message}", traceId, ex.Message);
                throw;
            }
        }

        public async Task<Job?> GetByIdAsync(string traceId, Guid jobId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Fetching job by Id through service", traceId);

                return await _jobRepository.GetByIdAsync(traceId, jobId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error fetching job by Id in service: {Message}", traceId, ex.Message);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(string traceId, Guid jobId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Checking job existence through service", traceId);

                return await _jobRepository.ExistsAsync(traceId, jobId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error checking job existence in service: {Message}", traceId, ex.Message);
                throw;
            }
        }

        public async Task AddJobAsync(string traceId, Job job, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Adding job through service: {JobId}", traceId, job.Id);

                await _jobRepository.AddAsync(traceId, job, cancellationToken);

                await NotifyAsync("JobAdded", job);

                _logger.LogInformation("{TraceId} - Job added successfully: {JobId}", traceId, job.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error adding job in service: {Message}", traceId, ex.Message);
                throw;
            }
        }
    }
}
