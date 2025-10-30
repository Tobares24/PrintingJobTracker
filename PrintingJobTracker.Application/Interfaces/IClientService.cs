using PrintingJobTracker.Application.DTOs;

namespace PrintingJobTracker.Application.Interfaces
{
    public interface IClientService
    {
        Task<List<ClientResponse>> GetClientsAsync(string traceId, ClientFilterRequest filter, CancellationToken cancellationToken = default);
    }
}
