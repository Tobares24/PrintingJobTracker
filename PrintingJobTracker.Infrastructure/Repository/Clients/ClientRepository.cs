using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PrintingJobTracker.Domain.Entities;
using PrintingJobTracker.Infrastructure.Persistence;
using System.Reflection;

namespace PrintingJobTracker.Infrastructure.Repository.Clients
{
    public sealed class ClientRepository(
        ILogger<ClientRepository> logger,
        DbContextFactoryService dbContextFactoryService) : IClientRepository
    {
        private readonly ILogger<ClientRepository> _logger = logger;
        private readonly DbContextFactoryService _dbContextFactoryService = dbContextFactoryService;

        public async Task<bool> ExistsAsync(string traceId, Guid clientId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Method invocation start: {Method}",
                    traceId, $"{MethodBase.GetCurrentMethod()!.ReflectedType!.FullName}.{MethodBase.GetCurrentMethod()!.Name}");

                bool existsClient = false;
                using (var dbContext = _dbContextFactoryService.CreateDbContext<ApplicationDbContext>())
                {
                    existsClient = await dbContext.Clients
                        .AsNoTracking()
                        .AnyAsync(c => c.Id == clientId && !c.IsDeleted, cancellationToken);
                }

                return existsClient;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error checking client existence: {Exception}", traceId, ex.Message);
                throw;
            }
            finally
            {
                _logger.LogInformation("{TraceId} - Method invocation end: {Method}",
                    traceId, $"{MethodBase.GetCurrentMethod()!.ReflectedType!.FullName}.{MethodBase.GetCurrentMethod()!.Name}");
            }
        }

        public async Task<Client?> GetByIdAsync(string traceId, Guid clientId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Method invocation start: {Method}",
                    traceId, $"{MethodBase.GetCurrentMethod()!.ReflectedType!.FullName}.{MethodBase.GetCurrentMethod()!.Name}");

                Client? client = null;
                using (var dbContext = _dbContextFactoryService.CreateDbContext<ApplicationDbContext>())
                {
                    client = await dbContext.Clients.FirstOrDefaultAsync(c => c.Id == clientId && !c.IsDeleted, cancellationToken);
                }

                return client;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error retrieving client: {Exception}", traceId, ex.Message);
                throw;
            }
            finally
            {
                _logger.LogInformation("{TraceId} - Method invocation end: {Method}",
                    traceId, $"{MethodBase.GetCurrentMethod()!.ReflectedType!.FullName}.{MethodBase.GetCurrentMethod()!.Name}");
            }
        }
    }
}
