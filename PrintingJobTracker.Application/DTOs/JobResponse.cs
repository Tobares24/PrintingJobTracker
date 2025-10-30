namespace PrintingJobTracker.Application.DTOs
{
    public sealed record JobResponse
   (
       Guid Id,
       string ClientName,
       string JobName,
       int Quantity,
       string Carrier,
       string CurrentStatus,
       DateTime CreatedAt
   );
}
