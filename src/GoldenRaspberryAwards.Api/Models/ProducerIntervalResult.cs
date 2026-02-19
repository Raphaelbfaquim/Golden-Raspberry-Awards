namespace GoldenRaspberryAwards.Api.Models;

public class ProducerIntervalResult
{
    public List<ProducerIntervalItem> Min { get; set; } = new();
    public List<ProducerIntervalItem> Max { get; set; } = new();
}

public class ProducerIntervalItem
{
    public string Producer { get; set; } = string.Empty;
    public int Interval { get; set; }
    public int PreviousWin { get; set; }
    public int FollowingWin { get; set; }
}
