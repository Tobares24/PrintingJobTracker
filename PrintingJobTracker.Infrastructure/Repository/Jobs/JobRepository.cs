using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PrintingJobTracker.Domain.Entities;
using PrintingJobTracker.Domain.Specifications;
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

        public async Task<List<Job>> GetJobsAsync(
            string traceId,
            ISpecification<Job> specification,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Start fetching jobs with specification", traceId);

                List<Job> jobs = [];
                using (var dbContext = _dbContextFactoryService.CreateDbContext<ApplicationDbContext>())
                {
                    IQueryable<Job> query = dbContext.Jobs
                        .Include(j => j.Client)
                        .AsNoTracking()
                        .Where(j => !j.IsDeleted);

                    if (specification.Criteria is not null)
                    {
                        query = query.Where(specification.Criteria);
                    }

                    query = query
                        .OrderByDescending(j => j.CreatedAt)
                        .ThenByDescending(j => j.Id)
                        .ThenByDescending(j => j.MailDeadline)
                        .Skip(specification.Skip)
                        .Take(specification.Take);

                    jobs = await query.ToListAsync(cancellationToken);
                }

                return jobs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error fetching jobs: {Exception}", traceId, ex.Message);
                throw;
            }
            finally
            {
                _logger.LogInformation("{TraceId} - End fetching jobs", traceId);
            }
        }

        public async Task<bool> ExistsAsync(string traceId, int jobId, CancellationToken cancellationToken = default)
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

        public async Task<Job?> GetByIdAsync(string traceId, int jobId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Start method {Method}",
                    traceId, $"{MethodBase.GetCurrentMethod()!.ReflectedType!.FullName}.{MethodBase.GetCurrentMethod()!.Name}");

                Job? job = null;
                using (var dbContext = _dbContextFactoryService.CreateDbContext<ApplicationDbContext>())
                {
                    job = await dbContext.Jobs
                        .Include(j => j.Client)
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

        public async Task<int> GetCountAsync(string traceId, ISpecification<Job> specification, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Getting job count from repository", traceId);

                using var dbContext = _dbContextFactoryService.CreateDbContext<ApplicationDbContext>();

                var query = dbContext.Jobs.AsQueryable();

                if (specification.Criteria is not null)
                    query = query.Where(specification.Criteria);

                return await query.CountAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error getting job count: {Message}", traceId, ex.Message);
                throw;
            }
        }

        public async Task UpdateAsync(string traceId, Job job, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Updating job: {JobId}", traceId, job.Id);

                using var dbContext = _dbContextFactoryService.CreateDbContext<ApplicationDbContext>();

                dbContext.Jobs.Update(job);
                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("{TraceId} - Job updated: {JobId}", traceId, job.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error updating job: {Message}", traceId, ex.Message);
                throw;
            }
        }

        public async Task AddHistoryAsync(string traceId, JobStatusHistory history, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Adding job history for job: {JobId}", traceId, history.JobId);

                using var dbContext = _dbContextFactoryService.CreateDbContext<ApplicationDbContext>();

                await dbContext.JobStatusHistories.AddAsync(history, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("{TraceId} - Job history added: {HistoryId}", traceId, history.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error adding job history: {Message}", traceId, ex.Message);
                throw;
            }
        }

        public async Task<List<JobStatusHistory>> GetJobHistoryAsync(string traceId, int jobId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Fetching job history for: {JobId}", traceId, jobId);

                using var dbContext = _dbContextFactoryService.CreateDbContext<ApplicationDbContext>();

                return await dbContext.JobStatusHistories
                    .Where(h => h.JobId == jobId)
                    .OrderByDescending(h => h.ChangedAt)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error fetching job history: {Message}", traceId, ex.Message);
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetJobCountsByStatusAsync(string traceId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Fetching job counts by status", traceId);

                using var dbContext = _dbContextFactoryService.CreateDbContext<ApplicationDbContext>();

                return await dbContext.Jobs
                    .GroupBy(j => j.CurrentStatus)
                    .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                    .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error fetching job counts by status: {Message}", traceId, ex.Message);
                throw;
            }
        }
    }
}
