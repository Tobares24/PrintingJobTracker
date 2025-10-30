using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PrintingJobTracker.Domain.Entities;
using PrintingJobTracker.Infrastructure.Persistence;
using System.Reflection;

namespace PrintingJobTracker.Infrastructure.Repository.Jobs
{
    public sealed class JobStatusHistoryRepository(
        ILogger<JobStatusHistoryRepository> logger,
        DbContextFactoryService dbContextFactoryService) : IJobStatusHistoryRepository
    {
        private readonly ILogger<JobStatusHistoryRepository> _logger = logger;
        private readonly DbContextFactoryService _dbContextFactoryService = dbContextFactoryService;

        public async Task<IReadOnlyList<JobStatusHistory>> GetJobStatusHistoriesPaginatedAsync(
            int jobId,
            int pageNumber,
            int pageSize,
            string traceId,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Starting execution of method {Method}",
                    traceId, $"{MethodBase.GetCurrentMethod()!.ReflectedType!.FullName}.{MethodBase.GetCurrentMethod()!.Name}");

                List<JobStatusHistory> histories = [];
                using (var dbContext = _dbContextFactoryService.CreateDbContext<ApplicationDbContext>())
                {
                    var query = dbContext.JobStatusHistories
                        .AsNoTracking()
                        .Where(h => h.JobId == jobId)
                        .OrderByDescending(h => h.ChangedAt);

                    histories = await query
                       .Skip((pageNumber - 1) * pageSize)
                       .Take(pageSize)
                       .ToListAsync(cancellationToken);
                }

                return histories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error while retrieving job status history. Exception: {Exception}",
                    traceId, ex.Message);
                throw;
            }
            finally
            {
                _logger.LogInformation("{TraceId} - Finished execution of method {Method}",
                    traceId, $"{MethodBase.GetCurrentMethod()!.ReflectedType!.FullName}.{MethodBase.GetCurrentMethod()!.Name}");
            }
        }

        public async Task AddAsync(
            string traceId,
            JobStatusHistory history,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Starting execution of method {Method}",
                    traceId, $"{MethodBase.GetCurrentMethod()!.ReflectedType!.FullName}.{MethodBase.GetCurrentMethod()!.Name}");

                using (var dbContext = _dbContextFactoryService.CreateDbContext<ApplicationDbContext>())
                {
                    await dbContext.JobStatusHistories.AddAsync(history, cancellationToken);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error while adding job status history Exception: {Exception}",
                    traceId, ex.Message);
                throw;
            }
            finally
            {
                _logger.LogInformation("{TraceId} - Finished execution of method {Method}",
                    traceId, $"{MethodBase.GetCurrentMethod()!.ReflectedType!.FullName}.{MethodBase.GetCurrentMethod()!.Name}");
            }
        }
    }
}
