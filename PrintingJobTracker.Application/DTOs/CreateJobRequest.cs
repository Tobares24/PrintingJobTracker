using PrintingJobTracker.Domain.Entities.Enums;

namespace PrintingJobTracker.Application.DTOs
{
    public class CreateJobRequest
    {
        public int ClientId { get; set; }
        public string? JobName { get; set; }
        public int Quantity { get; set; }
        public CarrierType Carrier { get; set; }
        public DateTime MailDeadline { get; set; }
    }

}
