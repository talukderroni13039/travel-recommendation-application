using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TravelRecommendation.Application.DTO;

namespace TravelRecommendation.Application.Interface
{
    public class AirQualityService : IAirQualityService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AirQualityService> _logger;

        public AirQualityService(IHttpClientFactory httpClientFactory, ILogger<AirQualityService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<AirQualityApiResponse> GetAirQualityAsync(double latitude, double longitude)
        {
            var client = _httpClientFactory.CreateClient("AirQuality");

            var url = $"v1/air-quality?latitude={latitude}&longitude={longitude}&hourly=pm2_5&forecast_days=7";

            _logger.LogDebug("Calling Air Quality API: {Url}", url);

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<AirQualityApiResponse>(json, options);
        }
    }
}
