using Microsoft.EntityFrameworkCore;
using GoldenRaspberryAwards.Api.Models;

namespace GoldenRaspberryAwards.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProducerWin> ProducerWins => Set<ProducerWin>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProducerWin>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.ProducerName).IsRequired();
            e.HasIndex(x => new { x.ProducerName, x.Year });
        });
    }
}
