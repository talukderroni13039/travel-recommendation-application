using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TravelRecommendation.Application.DTO;

namespace TravelRecommendation.Application.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(IHttpClientFactory httpClientFactory, ILogger<WeatherService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<WeatherApiResponse> GetWeatherForecastAsync(double latitude, double longitude)
        {
            var client = _httpClientFactory.CreateClient("OpenMeteo");

            var url = $"v1/forecast?latitude={latitude}&longitude={longitude}&hourly=temperature_2m&forecast_days=7";

            _logger.LogDebug("Calling Weather API: {Url}", url);

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<WeatherApiResponse>(json, options);
        }
    }
}
