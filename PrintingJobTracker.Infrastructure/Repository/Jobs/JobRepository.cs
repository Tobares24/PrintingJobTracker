using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PrintingJobTracker.Domain.Entities;
using PrintingJobTracker.Infrastructure.Persistence;
using System.Reflection;

namespace PrintingJobTracker.Infrastructure.Repository.Jobs
{
    public sealed class JobRepository(
        ILogger<JobRepository> logger,
        DbContextFactoryService dbContextFactoryService) : IJobRepository
    {
        private readonly ILogger<JobRepository> _logger = logger;
        private readonly DbContextFactoryService _dbContextFactoryService = dbContextFactoryService;

        public async Task<bool> ExistsAsync(string traceId, Guid jobId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Start method {Method}",
                    traceId, $"{MethodBase.GetCurrentMethod()!.ReflectedType!.FullName}.{MethodBase.GetCurrentMethod()!.Name}");

                bool exists = false;
                using (var dbContext = _dbContextFactoryService.CreateDbContext<ApplicationDbContext>())
                {
                    exists = await dbContext.Jobs
                        .AsNoTracking()
                        .AnyAsync(j => j.Id == jobId && !j.IsDeleted, cancellationToken);
                }

                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error checking job existence: {Exception}", traceId, ex.Message);
                throw;
            }
            finally
            {
                _logger.LogInformation("{TraceId} - End method {Method}",
                    traceId, $"{MethodBase.GetCurrentMethod()!.ReflectedType!.FullName}.{MethodBase.GetCurrentMethod()!.Name}");
            }
        }

        public async Task<Job?> GetByIdAsync(string traceId, Guid jobId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Start method {Method}",
                    traceId, $"{MethodBase.GetCurrentMethod()!.ReflectedType!.FullName}.{MethodBase.GetCurrentMethod()!.Name}");

                Job? job = null;
                using (var dbContext = _dbContextFactoryService.CreateDbContext<ApplicationDbContext>())
                {
                    job = await dbContext.Jobs
                        .FirstOrDefaultAsync(j => j.Id == jobId && !j.IsDeleted, cancellationToken);
                }

                return job;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error fetching job: {Exception}", traceId, ex.Message);
                throw;
            }
            finally
            {
                _logger.LogInformation("{TraceId} - End method {Method}",
                    traceId, $"{MethodBase.GetCurrentMethod()!.ReflectedType!.FullName}.{MethodBase.GetCurrentMethod()!.Name}");
            }
        }

        public async Task AddAsync(string traceId, Job job, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Start adding job {JobId}", traceId, job.Id);

                using (var dbContext = _dbContextFactoryService.CreateDbContext<ApplicationDbContext>())
                {
                    await dbContext.Jobs.AddAsync(job, cancellationToken);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }

                _logger.LogInformation("{TraceId} - Job {JobId} added successfully", traceId, job.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error adding job {JobId}: {Exception}", traceId, job.Id, ex.Message);
                throw;
            }
        }
    }
}
