public interface IWeatherService
{
    Task<string[]> GetWeatherInCity(string city, CancellationToken cancellationToken = default);
}

