using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GoldenRaspberryAwards.Api.Data;

namespace GoldenRaspberryAwards.Tests.Integration;

public class WebAppFactory : WebApplicationFactory<Program>
{
    private readonly string _csvPath;

    public WebAppFactory()
    {
        _csvPath = Path.Combine(AppContext.BaseDirectory, "Movielist.csv");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });
        });

        builder.UseSetting("CsvPath", _csvPath);
    }
}
