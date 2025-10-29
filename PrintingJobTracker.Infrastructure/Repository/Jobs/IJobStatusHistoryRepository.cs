using PrintingJobTracker.Domain.Entities;

namespace PrintingJobTracker.Infrastructure.Repository.Jobs
{
    public interface IJobStatusHistoryRepository
    {
        Task AddAsync(
            string traceId,
            JobStatusHistory history,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<JobStatusHistory>> GetJobStatusHistoriesPaginatedAsync(
            Guid jobId,
            int pageNumber,
            int pageSize,
            string traceId,
            CancellationToken cancellationToken);
    }
}
