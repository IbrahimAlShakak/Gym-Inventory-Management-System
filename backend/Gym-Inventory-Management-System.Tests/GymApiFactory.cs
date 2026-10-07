using Gym_Inventory_Management_System.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Gym_Inventory_Management_System.Tests;

public class GymApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();

        // Tests must not depend on your personal User Secrets.
        builder.UseSetting("Jwt:Key", "db13434e1523f52612335b48d085b7ae3c7ea248c70ab3cfe7f9e0cd3fb0f09f");

        builder.ConfigureTestServices(services =>
        {
            // Remove everything the app registered for AppDbContext (the SQL Server setup).
            var toRemove = services.Where(d =>
                d.ServiceType == typeof(AppDbContext) ||
                (d.ServiceType.IsGenericType &&
                 d.ServiceType.GenericTypeArguments.Contains(typeof(AppDbContext))))
                .ToList();
            foreach (var descriptor in toRemove) services.Remove(descriptor);

            // Register the same DbContext, this time on SQLite.
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));

            // Create the tables now, because Program.cs seeds data right after startup.
            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) _connection.Dispose();
    }
}

