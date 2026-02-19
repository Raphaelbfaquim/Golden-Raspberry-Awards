using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using GoldenRaspberryAwards.Api.Models;
using GoldenRaspberryAwards.Api.Services;

namespace GoldenRaspberryAwards.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[EnableRateLimiting("fixed")]
public class ProducersController : ControllerBase
{
    private readonly IProducerIntervalService _intervalService;

    public ProducersController(IProducerIntervalService intervalService)
    {
        _intervalService = intervalService;
    }

    /// <summary>
    /// Retorna produtores com menor e maior intervalo entre dois prêmios consecutivos (Pior Filme).
    /// </summary>
    [HttpGet("intervals")]
    [ProducesResponseType(typeof(ProducerIntervalResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProducerIntervalResult>> GetIntervals(CancellationToken cancellationToken)
    {
        var result = await _intervalService.GetMinMaxIntervalsAsync();
        return Ok(result);
    }
}
