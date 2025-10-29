using PrintingJobTracker.Domain.Entities.Enums;

namespace PrintingJobTracker.Domain.Entities
{
    public class JobStatusHistory
    {
        public int Id { get; set; }

        public int JobId { get; set; }
        public Job Job { get; set; } = null!;

        public JobStatus Status { get; set; }

        public string? Note { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
