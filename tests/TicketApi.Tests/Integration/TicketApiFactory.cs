using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public class TicketApiFactory : WebApplicationFactory<Program>
{

    private readonly SqliteConnection _connection = new("Data Source=:memory:");

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _connection.Dispose();

        base.Dispose(disposing);
    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {

        _connection.Open();
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<TicketDbContext>>();
            services.AddDbContext<TicketDbContext>(options =>
                options.UseSqlite(_connection));

            using var scope = services.BuildServiceProvider().CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<TicketDbContext>();

            dbContext.Database.EnsureCreated();
        });

        base.ConfigureWebHost(builder);
    }
}