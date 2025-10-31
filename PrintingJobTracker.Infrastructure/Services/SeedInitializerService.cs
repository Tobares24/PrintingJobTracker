using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using PrintingJobTracker.Domain.Entities;
using PrintingJobTracker.Domain.Entities.Enums;
using PrintingJobTracker.Infrastructure.Persistence;

namespace PrintingJobTracker.Infrastructure.Services
{
    public class SeedInitializerService
    {
        private const int InitSeedFlag = 1;
        private readonly ILogger<SeedInitializerService> _logger;
        private readonly DbContextFactoryService _dbContextFactoryService;
        private readonly IConfiguration _configuration;

        public SeedInitializerService(
            ILogger<SeedInitializerService> logger,
            DbContextFactoryService dbContextFactoryService,
            IConfiguration configuration)
        {
            _logger = logger;
            _dbContextFactoryService = dbContextFactoryService;
            _configuration = configuration;
        }

        // Checks if a seed flag is enabled in ApplicationSettings
        private bool ShouldSeed(string key) =>
            int.Parse(_configuration[$"ApplicationSettings:{key}"] ?? "0") == InitSeedFlag;

        // Seeds initial clients if none exist
        public async Task SeedClientsAsync()
        {
            if (!ShouldSeed("INIT_SEED_CLIENTS")) return;

            using var dbContext = _dbContextFactoryService.CreateDbContext<ApplicationDbContext>();
            if (await dbContext.Clients.AnyAsync()) return;

            _logger.LogInformation("Seeding 10 Clients...");

            var clients = new List<Client>
            {
                new() { FirstName = "John", LastName = "Doe", IdentityCard = "123456" },
                new() { FirstName = "Jane", LastName = "Smith", IdentityCard = "654321" },
                new() { FirstName = "Alice", LastName = "Johnson", IdentityCard = "987654" },
                new() { FirstName = "Michael", LastName = "Brown", IdentityCard = "112233" },
                new() { FirstName = "Emily", LastName = "Davis", IdentityCard = "445566" },
                new() { FirstName = "Daniel", LastName = "Miller", IdentityCard = "778899" },
                new() { FirstName = "Sophia", LastName = "Wilson", IdentityCard = "223344" },
                new() { FirstName = "William", LastName = "Moore", IdentityCard = "556677" },
                new() { FirstName = "Olivia", LastName = "Taylor", IdentityCard = "889900" },
                new() { FirstName = "James", LastName = "Anderson", IdentityCard = "334455" }
            };

            await dbContext.Clients.AddRangeAsync(clients);
            await dbContext.SaveChangesAsync();
        }

        // Seeds initial jobs if none exist
        public async Task SeedJobsAsync()
        {
            if (!ShouldSeed("INIT_SEED_JOBS")) return;

            using var dbContext = _dbContextFactoryService.CreateDbContext<ApplicationDbContext>();
            if (await dbContext.Jobs.AnyAsync()) return;

            _logger.LogInformation("Seeding 10 Jobs...");

            var clients = await dbContext.Clients.Take(10).ToListAsync();
            var jobs = new List<Job>
            {
                new() { ClientId = clients[0].Id, JobName = "Print Brochure", Quantity = 100, Carrier = CarrierType.UPS, MailDeadline = DateTime.UtcNow.AddDays(5) },
                new() { ClientId = clients[1].Id, JobName = "Business Cards", Quantity = 250, Carrier = CarrierType.FedEx, MailDeadline = DateTime.UtcNow.AddDays(3) },
                new() { ClientId = clients[2].Id, JobName = "Flyers", Quantity = 500, Carrier = CarrierType.UPS, MailDeadline = DateTime.UtcNow.AddDays(7) },
                new() { ClientId = clients[3].Id, JobName = "Catalogs", Quantity = 150, Carrier = CarrierType.FedEx, MailDeadline = DateTime.UtcNow.AddDays(10) },
                new() { ClientId = clients[4].Id, JobName = "Letterheads", Quantity = 300, Carrier = CarrierType.UPS, MailDeadline = DateTime.UtcNow.AddDays(4) },
                new() { ClientId = clients[5].Id, JobName = "Envelopes", Quantity = 400, Carrier = CarrierType.FedEx, MailDeadline = DateTime.UtcNow.AddDays(6) },
                new() { ClientId = clients[6].Id, JobName = "Stickers", Quantity = 800, Carrier = CarrierType.UPS, MailDeadline = DateTime.UtcNow.AddDays(2) },
                new() { ClientId = clients[7].Id, JobName = "Posters", Quantity = 200, Carrier = CarrierType.FedEx, MailDeadline = DateTime.UtcNow.AddDays(8) },
                new() { ClientId = clients[8].Id, JobName = "Notepads", Quantity = 150, Carrier = CarrierType.UPS, MailDeadline = DateTime.UtcNow.AddDays(5) },
                new() { ClientId = clients[9].Id, JobName = "Calendars", Quantity = 100, Carrier = CarrierType.FedEx, MailDeadline = DateTime.UtcNow.AddDays(9) }
            };

            await dbContext.Jobs.AddRangeAsync(jobs);
            await dbContext.SaveChangesAsync();
        }

        // Seeds initial job status history if none exist
        public async Task SeedJobStatusHistoryAsync()
        {
            if (!ShouldSeed("INIT_SEED_JOBSTATUS")) return;

            using var dbContext = _dbContextFactoryService.CreateDbContext<ApplicationDbContext>();
            if (await dbContext.JobStatusHistories.AnyAsync()) return;

            _logger.LogInformation("Seeding 10 JobStatusHistory entries...");

            var jobs = await dbContext.Jobs.Take(10).ToListAsync();
            var histories = jobs.Select(job => new JobStatusHistory
            {
                JobId = job.Id,
                Status = JobStatus.Received,
                ChangedAt = DateTime.UtcNow
            }).ToList();

            await dbContext.JobStatusHistories.AddRangeAsync(histories);
            await dbContext.SaveChangesAsync();
        }

        // Runs all seeds sequentially
        public async Task SeedAllAsync()
        {
            _logger.LogInformation("Starting full seed process...");

            await SeedClientsAsync();
            await SeedJobsAsync();
            await SeedJobStatusHistoryAsync();

            _logger.LogInformation("Seeding completed.");
        }
    }
}
