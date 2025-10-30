namespace PrintingJobTracker.Application.DTOs
{
    public sealed record JobFilterRequest(
        string? JobName,
        DateTime? MailDeadlineFrom,
        DateTime? MailDeadlineTo,
        int Skip = 0,
        int Take = 10
    );
}
