using PrintingJobTracker.Domain.Entities.Enums;

namespace PrintingJobTracker.Domain.Entities
{
    public class JobStatusHistory
    {
        public Guid Id { get; set; }

        public Guid JobId { get; set; }
        public Job? Job { get; set; } 

        public JobStatus Status { get; set; }

        public string? Note { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
