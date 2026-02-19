using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using GoldenRaspberryAwards.Api.Data;
using GoldenRaspberryAwards.Api.Models;

namespace GoldenRaspberryAwards.Api.Services;

public class CsvLoaderService : ICsvLoaderService
{
    private readonly AppDbContext _db;

    public CsvLoaderService(AppDbContext db)
    {
        _db = db;
    }

    public async Task LoadIfEmptyAsync(string csvPath)
    {
        if (await _db.ProducerWins.AnyAsync())
            return;

        var fullPath = Path.IsPathRooted(csvPath)
            ? csvPath
            : Path.Combine(AppContext.BaseDirectory, csvPath);

        if (!File.Exists(fullPath))
            return;

        var lines = await File.ReadAllLinesAsync(fullPath);
        if (lines.Length < 2)
            return;

        var delimiter = lines[0].Contains(';') ? ';' : ',';
        var headers = ParseCsvLine(lines[0], delimiter);
        var yearIdx = FindColumnIndex(headers, "year");
        var producersIdx = FindColumnIndex(headers, "producers");
        var winnerIdx = FindColumnIndex(headers, "winner");

        if (yearIdx < 0 || producersIdx < 0 || winnerIdx < 0)
            return;

        var wins = new List<ProducerWin>();
        for (var i = 1; i < lines.Length; i++)
        {
            var cols = ParseCsvLine(lines[i], delimiter);
            if (cols.Count <= Math.Max(yearIdx, Math.Max(producersIdx, winnerIdx)))
                continue;

            var winnerCell = cols[winnerIdx].Trim();
            var isWinner = winnerCell.Equals("yes", StringComparison.OrdinalIgnoreCase)
                || winnerCell.Equals("true", StringComparison.OrdinalIgnoreCase)
                || winnerCell.Equals("1");

            if (!isWinner)
                continue;

            if (!int.TryParse(cols[yearIdx].Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var year))
                continue;

            var producersRaw = cols[producersIdx].Trim();
            var producerNames = SplitProducers(producersRaw);
            foreach (var name in producerNames)
            {
                if (string.IsNullOrWhiteSpace(name))
                    continue;
                wins.Add(new ProducerWin { ProducerName = name.Trim(), Year = year });
            }
        }

        if (wins.Count > 0)
        {
            await _db.ProducerWins.AddRangeAsync(wins);
            await _db.SaveChangesAsync();
        }
    }

    private static List<string> ParseCsvLine(string line, char delimiter = ',')
    {
        var list = new List<string>();
        var current = "";
        var inQuotes = false;
        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (c == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }
            if (!inQuotes && c == delimiter)
            {
                list.Add(current);
                current = "";
                continue;
            }
            current += c;
        }
        list.Add(current);
        return list;
    }

    private static int FindColumnIndex(List<string> headers, string name)
    {
        var n = name.ToLowerInvariant();
        for (var i = 0; i < headers.Count; i++)
            if (headers[i].Trim().ToLowerInvariant() == n)
                return i;
        return -1;
    }

    /// <summary>
    /// Split producers by comma and " and ", trim.
    /// </summary>
    private static IEnumerable<string> SplitProducers(string raw)
    {
        var normalized = Regex.Replace(raw, @"\s+and\s+", ",", RegexOptions.IgnoreCase);
        foreach (var part in normalized.Split(','))
        {
            var t = part.Trim();
            if (t.Length > 0)
                yield return t;
        }
    }
}
