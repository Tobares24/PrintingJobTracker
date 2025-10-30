using PrintingJobTracker.Domain.Entities;
using PrintingJobTracker.Domain.Specifications;

namespace PrintingJobTracker.Infrastructure.Repository.Jobs
{
    public interface IJobRepository
    {
        Task<bool> ExistsAsync(string traceId, Guid jobId, CancellationToken cancellationToken = default);
        Task<Job?> GetByIdAsync(string traceId, Guid jobId, CancellationToken cancellationToken = default);
        Task AddAsync(string traceId, Job job, CancellationToken cancellationToken = default);
        Task<List<Job>> GetJobsAsync(string traceId, ISpecification<Job> specification, CancellationToken cancellationToken = default);
    }
}
