using Microsoft.EntityFrameworkCore;
using GoldenRaspberryAwards.Api.Data;
using GoldenRaspberryAwards.Api.Models;

namespace GoldenRaspberryAwards.Api.Services;

public class ProducerIntervalService : IProducerIntervalService
{
    private readonly AppDbContext _db;

    public ProducerIntervalService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ProducerIntervalResult> GetMinMaxIntervalsAsync()
    {
        var allWins = await _db.ProducerWins.Select(x => new { x.ProducerName, x.Year }).ToListAsync();
        var byProducer = allWins
            .GroupBy(x => x.ProducerName)
            .Select(g => new { Producer = g.Key, Years = g.Select(x => x.Year).Distinct().OrderBy(y => y).ToList() })
            .ToList();

        var allIntervals = new List<ProducerIntervalItem>();
        foreach (var p in byProducer)
        {
            var years = p.Years;
            if (years.Count < 2)
                continue;
            for (var i = 0; i < years.Count - 1; i++)
            {
                var prev = years[i];
                var next = years[i + 1];
                allIntervals.Add(new ProducerIntervalItem
                {
                    Producer = p.Producer,
                    Interval = next - prev,
                    PreviousWin = prev,
                    FollowingWin = next
                });
            }
        }

        if (allIntervals.Count == 0)
            return new ProducerIntervalResult();

        var minInterval = allIntervals.Min(x => x.Interval);
        var maxInterval = allIntervals.Max(x => x.Interval);

        return new ProducerIntervalResult
        {
            Min = allIntervals.Where(x => x.Interval == minInterval).ToList(),
            Max = allIntervals.Where(x => x.Interval == maxInterval).ToList()
        };
    }
}
