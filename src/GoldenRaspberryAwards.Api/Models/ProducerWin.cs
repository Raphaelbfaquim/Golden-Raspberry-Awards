namespace GoldenRaspberryAwards.Api.Models;

/// <summary>
/// Representa uma vitória de um produtor em um ano (Pior Filme - Golden Raspberry).
/// Uma linha por produtor por filme vencedor.
/// </summary>
public class ProducerWin
{
    public int Id { get; set; }
    public string ProducerName { get; set; } = string.Empty;
    public int Year { get; set; }
}
