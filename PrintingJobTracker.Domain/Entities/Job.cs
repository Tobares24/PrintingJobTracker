using PrintingJobTracker.Domain.Entities.Enums;

namespace PrintingJobTracker.Domain.Entities
{
    public class Job
    {
        public int Id { get; private set; }

        public int ClientId { get; set; }

        public virtual Client? Client { get; set; }

        public string? JobName { get; set; }

        public int Quantity { get; set; }

        public CarrierType Carrier { get; set; }

        public JobStatus CurrentStatus { get; set; } = JobStatus.Received;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime MailDeadline { get; set; }

        public bool IsDeleted { get; set; }

        public virtual ICollection<JobStatusHistory>? StatusHistory { get; set; } = new List<JobStatusHistory>();
    }
}
