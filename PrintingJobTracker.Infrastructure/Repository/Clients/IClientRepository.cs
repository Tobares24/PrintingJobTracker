using PrintingJobTracker.Domain.Entities;

namespace PrintingJobTracker.Infrastructure.Repository.Clients
{
    public interface IClientRepository
    {
        Task<bool> ExistsAsync(string traceId, int clientId, CancellationToken cancellationToken = default);
        Task<Client?> GetByIdAsync(string traceId, int clientId, CancellationToken cancellationToken = default);
        Task<List<Client>> GetClientsAsync(string traceId, string? filter, CancellationToken cancellationToken = default);
    }
}
