namespace PrintingJobTracker.Application.DTOs
{
    public sealed class ClientResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
