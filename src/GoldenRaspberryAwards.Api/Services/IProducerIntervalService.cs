using GoldenRaspberryAwards.Api.Models;

namespace GoldenRaspberryAwards.Api.Services;

public interface IProducerIntervalService
{
    /// <summary>
    /// Retorna produtores com menor e maior intervalo entre dois prêmios consecutivos.
    /// </summary>
    Task<ProducerIntervalResult> GetMinMaxIntervalsAsync();
}
