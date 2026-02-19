using System.Net;
using System.Net.Http.Json;
using GoldenRaspberryAwards.Api.Models;
using Xunit;

namespace GoldenRaspberryAwards.Tests.Integration;

public class ProducersControllerIntegrationTests : IClassFixture<WebAppFactory>
{
    private readonly WebAppFactory _factory;

    public ProducersControllerIntegrationTests(WebAppFactory factory)
    {
        _factory = factory;
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = _factory.CreateClient();
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            UserName = "testuser",
            Password = "testpass"
        });
        loginResponse.EnsureSuccessStatusCode();
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(loginResult?.Token);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);
        return client;
    }

    [Fact]
    public async Task GetIntervals_WithoutToken_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/producers/intervals");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetIntervals_Returns200_AndMinMaxStructure()
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.GetAsync("/api/producers/intervals");
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ProducerIntervalResult>();
        Assert.NotNull(result);
        Assert.NotNull(result.Min);
        Assert.NotNull(result.Max);
    }

    [Fact]
    public async Task GetIntervals_ReturnsDataAccordingToMovielistCsv()
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.GetAsync("/api/producers/intervals");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ProducerIntervalResult>();
        Assert.NotNull(result);

        // Movielist.csv: menor intervalo = Joel Silver (1990 e 1991, intervalo 1). Maior = Matthew Vaughn (2002 e 2015, intervalo 13).
        Assert.NotEmpty(result.Min);
        var minItem = result.Min.First();
        Assert.Equal("Joel Silver", minItem.Producer);
        Assert.Equal(1, minItem.Interval);
        Assert.Equal(1990, minItem.PreviousWin);
        Assert.Equal(1991, minItem.FollowingWin);

        Assert.NotEmpty(result.Max);
        var maxItem = result.Max.First();
        Assert.Equal("Matthew Vaughn", maxItem.Producer);
        Assert.Equal(13, maxItem.Interval);
        Assert.Equal(2002, maxItem.PreviousWin);
        Assert.Equal(2015, maxItem.FollowingWin);
    }
}
