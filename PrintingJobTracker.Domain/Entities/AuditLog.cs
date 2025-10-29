namespace PrintingJobTracker.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string EntityName { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty;
        public string? NewRecord { get; set; }
        public string? PreviousRecord { get; set; }
        public DateTime OcurredOn { get; set; } = DateTime.UtcNow;
    }
}
