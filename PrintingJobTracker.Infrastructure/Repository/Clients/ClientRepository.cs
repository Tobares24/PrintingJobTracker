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

        public async Task<List<Client>> GetClientsAsync(string traceId, string? filter, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Method invocation start: {Method}",
                    traceId, $"{MethodBase.GetCurrentMethod()!.ReflectedType!.FullName}.{MethodBase.GetCurrentMethod()!.Name}");

                using (var dbContext = _dbContextFactoryService.CreateDbContext<ApplicationDbContext>())
                {
                    var query = dbContext.Clients.AsQueryable();

                    if (!string.IsNullOrWhiteSpace(filter))
                    {
                        query = query.Where(c =>
                            (c.FirstName != null && c.FirstName.StartsWith(filter)) ||
                            (c.LastName != null && c.LastName.StartsWith(filter)) ||
                            (c.SecondLastName != null && c.SecondLastName.StartsWith(filter))
                        );
                    }

                    var clients = await query
                        .OrderBy(c => c.FirstName)
                            .ThenBy(c => c.LastName)
                            .ThenBy(c => c.SecondLastName)
                        .Take(10)
                        .Select(c => new Client { Id = c.Id, FirstName = c.FirstName })
                        .ToListAsync(cancellationToken);

                    return clients;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error fetching clients: {Exception}", traceId, ex.Message);
                throw;
            }
            finally
            {
                _logger.LogInformation("{TraceId} - Method invocation end: {Method}",
                    traceId, $"{MethodBase.GetCurrentMethod()!.ReflectedType!.FullName}.{MethodBase.GetCurrentMethod()!.Name}");
            }
        }

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
