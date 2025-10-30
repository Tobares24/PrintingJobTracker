using Microsoft.Extensions.Logging;
using PrintingJobTracker.Application.DTOs;
using PrintingJobTracker.Application.Interfaces;
using PrintingJobTracker.Domain.Entities;
using PrintingJobTracker.Infrastructure.Repository.Clients;
using System.Reflection;

namespace PrintingJobTracker.Application.Services
{
    public class ClientService(ILogger<JobService> logger, IClientRepository clientRepository) : IClientService
    {
        private readonly ILogger<JobService> _logger = logger;
        private readonly IClientRepository _clientRepository = clientRepository;

        public async Task<List<ClientResponse>> GetClientsAsync(string traceId, ClientFilterRequest filter, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("{TraceId} - Method invocation start: {Method}",
                    traceId, $"{MethodBase.GetCurrentMethod()!.ReflectedType!.FullName}.{MethodBase.GetCurrentMethod()!.Name}");

                List<Client> clients = await _clientRepository.GetClientsAsync(traceId, filter.SearchTerm, cancellationToken);

                var response = clients.Select(c => new ClientResponse
                {
                    Id = c.Id,
                    Name = $"{c.FirstName} {c.LastName} {c.SecondLastName}".Trim()
                }).ToList();

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{TraceId} - Error fetching client responses: {Exception}", traceId, ex.Message);
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
