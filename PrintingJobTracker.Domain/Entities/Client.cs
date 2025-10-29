namespace PrintingJobTracker.Domain.Entities
{
    public class Client
    {
        public Guid Id { get; set; }

        public string? IdentityCard { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? SecondLastName { get; set; }

        public bool IsDeleted { get; set; }

        public virtual ICollection<Job>? Jobs { get; set; } = new List<Job>();
    }
}
