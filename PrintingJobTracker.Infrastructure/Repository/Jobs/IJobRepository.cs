using PrintingJobTracker.Domain.Entities;
using PrintingJobTracker.Domain.Specifications;

namespace PrintingJobTracker.Infrastructure.Repository.Jobs;

public interface IJobRepository
{
    Task AddAsync(string traceId, Job job, CancellationToken cancellationToken = default);
    Task<List<Job>> GetJobsAsync(string traceId, ISpecification<Job> specification, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(string traceId, ISpecification<Job> specification, CancellationToken cancellationToken = default);
    Task<Job?> GetByIdAsync(string traceId, int jobId, CancellationToken cancellationToken = default);
    Task UpdateAsync(string traceId, Job job, CancellationToken cancellationToken = default);
    Task AddHistoryAsync(string traceId, JobStatusHistory history, CancellationToken cancellationToken = default);
    Task<List<JobStatusHistory>> GetJobHistoryAsync(string traceId, int jobId, CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetJobCountsByStatusAsync(string traceId, CancellationToken cancellationToken = default);
}