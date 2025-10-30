using PrintingJobTracker.Domain.Entities.Enums;

namespace PrintingJobTracker.Application.DTOs
{
    public sealed record JobFilterRequest(
        string? JobName,
        JobStatus? Status,
        int Skip = 0,
        int Take = 10
    );
}
