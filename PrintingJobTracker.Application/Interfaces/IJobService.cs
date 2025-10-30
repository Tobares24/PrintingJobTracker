using PrintingJobTracker.Application.DTOs;
using PrintingJobTracker.Domain.Entities;

namespace PrintingJobTracker.Application.Interfaces
{
    public interface IJobService
    {
        Task<JobResponse> GetJobsAsync(string traceId, JobFilterRequest request, CancellationToken cancellationToken = default);
        Task<Job?> GetByIdAsync(string traceId, int jobId, CancellationToken cancellationToken = default);
        Task AddJobAsync(string traceId, CreateJobRequest request, CancellationToken cancellationToken = default);
        Task<bool> AdvanceJobStatusAsync(string traceId, int jobId, CancellationToken cancellationToken = default);
        Task<bool> SetJobExceptionAsync(string traceId, int jobId, string note, CancellationToken cancellationToken = default);
        Task<Dictionary<string, int>> GetJobCountsByStatusAsync(string traceId, CancellationToken cancellationToken = default);
        Task<List<JobStatusHistory>> GetJobHistoryAsync(string traceId, int jobId, CancellationToken cancellationToken = default);
    }
}
