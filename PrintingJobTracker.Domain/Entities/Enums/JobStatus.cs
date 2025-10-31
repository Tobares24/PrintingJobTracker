namespace PrintingJobTracker.Domain.Entities.Enums
{
    public enum JobStatus
    {
        Received,
        Printing,
        Inserting,
        Mailed,
        Delivered,
        Exception
    }
}
