using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BasicBilling.API.Infrastructure.Data;
using BasicBilling.API.Domain.Entities;
using BasicBilling.API.Domain.Enums;

namespace BasicBilling.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<BillingDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<BillingDbContext>(options =>
                options.UseSqlite(_connection));

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BillingDbContext>();
            db.Database.EnsureCreated();

            SeedTestData(db);
        });

        builder.UseEnvironment("Development");
    }

    private static void SeedTestData(BillingDbContext db)
    {
        var bill = db.Bills.Find(1);
        bill!.Status = BillStatus.Paid;

        db.Payments.Add(new Payment
        {
            Id = 1,
            BillId = 1,
            Amount = bill.Amount,
            PaymentDate = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc)
        });

        db.SaveChanges();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection?.Dispose();
        }
    }
}
