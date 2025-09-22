
using System.Text.Json;

namespace ConsoleAgent.Services;

public class WeatherApiDotComService(string apiKey) : IWeatherService
{
    private readonly HttpClient _httpClient = new();

    public async Task<string[]> GetWeatherInCity(string city, CancellationToken cancellationToken = default)
    {
        var url = $"https://api.weatherapi.com/v1/current.json?key={apiKey}&q={Uri.EscapeDataString(city)}&aqi=no";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Error calling Weather API: {response.StatusCode} - {responseContent}");

        using var doc = JsonDocument.Parse(responseContent);
        var root = doc.RootElement;
        var descriptionElement = root.GetProperty("current").GetProperty("condition").GetProperty("text");

        string[] descriptions = [descriptionElement.GetString()!];

        return descriptions;
    }
}

