using PrintingJobTracker.Domain.Entities.Enums;

namespace PrintingJobTracker.Application.DTOs
{
    public sealed record JobResponse(
         List<JobModel> Jobs,
         int TotalCount
     );

    public sealed record JobModel(
       int Id,
       string ClientName,
       string JobName,
       int Quantity,
       string Carrier,
       JobStatus CurrentStatus,
       DateTime CreatedAt
    );
}
