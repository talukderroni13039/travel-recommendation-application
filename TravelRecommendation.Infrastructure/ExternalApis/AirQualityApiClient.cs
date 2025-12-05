using Microsoft.Extensions.Logging;
using System.Text.Json;
using TravelRecommendation.Application.DTO.Responses;
using TravelRecommendation.Application.Interface.ExternalApis;

namespace TravelRecommendation.Infrastructure.ExternalApis
{
    public class AirQualityApiClient : IAirQualityApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AirQualityApiClient> _logger;

        public AirQualityApiClient(IHttpClientFactory httpClientFactory, ILogger<AirQualityApiClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<AirQualityApiResponse> GetAirQualityAsync(double latitude, double longitude,string startDate, string endDate)
        {

            try
            {

           


            var client = _httpClientFactory.CreateClient("AirQuality");
            var url = $"v1/air-quality?latitude={latitude}&longitude={longitude}&hourly=pm2_5&start_date={startDate}&end_date={endDate}";

            //var url = $"v1/air-quality?latitude={latitude}&longitude={longitude}&hourly=pm2_5&forecast_days=16";

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
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
