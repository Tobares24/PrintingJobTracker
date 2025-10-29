using PrintingJobTracker.Domain.Entities.Enums;

namespace PrintingJobTracker.Domain.Entities
{
    public class Job
    {
        public int Id { get; set; }

        public string ClientName { get; set; } = string.Empty;

        public string JobName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public CarrierType Carrier { get; set; }

        public JobStatus CurrentStatus { get; set; } = JobStatus.Received;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime SLA_MailBy { get; set; }

        public virtual ICollection<JobStatusHistory> StatusHistory { get; set; } = new List<JobStatusHistory>();
    }
}
