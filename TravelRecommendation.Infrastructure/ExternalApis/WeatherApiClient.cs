using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TravelRecommendation.Application.DTO;
using TravelRecommendation.Application.Services;

namespace TravelRecommendation.Infrastructure.ExternalApis
{
    public class WeatherApiClient : IWeatherApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WeatherApiClient> _logger;

        public WeatherApiClient(IHttpClientFactory httpClientFactory, ILogger<WeatherApiClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<WeatherApiResponse> GetWeatherForecastAsync(double latitude, double longitude, string startDate,string endDate)
        {
            var client = _httpClientFactory.CreateClient("OpenMeteo");

            var url = $"v1/forecast?latitude={latitude}&longitude={longitude}&hourly=temperature_2m&start_date={startDate}&end_date={endDate}";

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
