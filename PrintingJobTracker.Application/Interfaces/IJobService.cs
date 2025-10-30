using PrintingJobTracker.Application.DTOs;
using PrintingJobTracker.Domain.Entities;

namespace PrintingJobTracker.Application.Interfaces
{
    public interface IJobService
    {
        Task<List<Job>> GetJobsAsync(string traceId, JobFilterRequest request, CancellationToken cancellationToken = default);
        Task<Job?> GetByIdAsync(string traceId, Guid jobId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string traceId, Guid jobId, CancellationToken cancellationToken = default);
        Task AddJobAsync(string traceId, Job job, CancellationToken cancellationToken = default);
    }
}
