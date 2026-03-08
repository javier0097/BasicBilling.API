using Microsoft.EntityFrameworkCore;
using BasicBilling.API.Domain.Entities;
using BasicBilling.API.Domain.Enums;

namespace BasicBilling.API.Infrastructure.Data;

public class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Bill> Bills => Set<Bill>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relationships
        modelBuilder.Entity<Client>()
            .HasMany(c => c.Bills)
            .WithOne(b => b.Client)
            .HasForeignKey(b => b.ClientId);

        modelBuilder.Entity<Bill>()
            .HasOne(b => b.Payment)
            .WithOne(p => p.Bill)
            .HasForeignKey<Payment>(p => p.BillId);

        // Store ServiceType enum as string in the database
        modelBuilder.Entity<Bill>()
            .Property(b => b.ServiceType)
            .HasConversion<string>();

        // Seed Clients
        modelBuilder.Entity<Client>().HasData(
            new Client { Id = 100, Name = "Joseph Carlton" },
            new Client { Id = 200, Name = "Maria Juarez" },
            new Client { Id = 300, Name = "Albert Kenny" },
            new Client { Id = 400, Name = "Jessica Phillips" },
            new Client { Id = 500, Name = "Charles Johnson" }
        );

        // Seed Bills: 5 clients x 3 services x 2 months = 30 bills
        var bills = new List<Bill>();
        int billId = 1;
        int[] clientIds = { 100, 200, 300, 400, 500 };
        string[] periods = { "202602", "202603" };
        var serviceTypes = new[] { ServiceType.Water, ServiceType.Electricity, ServiceType.Sewer };
        var random = new Random(42); // Fixed seed for consistent amounts

        foreach (var clientId in clientIds)
        {
            foreach (var period in periods)
            {
                foreach (var serviceType in serviceTypes)
                {
                    bills.Add(new Bill
                    {
                        Id = billId++,
                        ClientId = clientId,
                        ServiceType = serviceType,
                        Period = period,
                        Amount = Math.Round((decimal)(random.NextDouble() * 150 + 10), 2),
                        Status = "Pending"
                    });
                }
            }
        }

        modelBuilder.Entity<Bill>().HasData(bills);
    }
}
