using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using TravelRecommendation.Application.DTO;
using TravelRecommendation.Application.Interface;
using TravelRecommendation.Domain;

namespace TravelRecommendation.Application.Services
{
    public class DistrictService : IDistrictService
    {
        private readonly IDistrictRepository _districtRepository;
        private readonly ILogger<DistrictService> _logger;
        private readonly IWeatherApiClient _weatherService;
        private readonly IAirQualityApiClient _airQualityService;

        public DistrictService(IDistrictRepository districtRepository, ILogger<DistrictService> logger, IWeatherApiClient weatherService, IAirQualityApiClient airQualityService)
        {
            _logger = logger;
            _weatherService = weatherService;
            _airQualityService = airQualityService;
            _districtRepository=districtRepository; 

        }
        public async Task<Top10Response> GetTop10DistrictsAsync()
        {
            _logger.LogInformation("Starting to calculate Top 10 districts...");

            var _districts = _districtRepository.GetAllDistricts();

            var districtsWeather = new ConcurrentBag<DistrictWeatherSummary>();
            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = 5
            };

            await Parallel.ForEachAsync(_districts, options, async (district, token) =>
            {
                DistrictWeatherSummary districtWeather = await ProcessDistrictAsync(district);
                if (districtWeather != null)
                {
                    districtsWeather.Add(districtWeather);
                }
            });

            _logger.LogInformation("Successfully processed {Count} districts", districtsWeather.Count);

            // Sort by temperature (ascending), then by PM2.5 (ascending) for ties
            var top10 = districtsWeather
                .Select(d => new
                {
                    d.DistrictName,
                    AvgTemperature = Math.Round(d.AvgTemperature, 1),
                    AvgPm25 = Math.Round(d.AvgPm25, 1)
                })
                .OrderBy(d => d.AvgTemperature)    // Sort by rounded temperature
                .ThenBy(d => d.AvgPm25)            // Then by rounded PM2.5
                .Take(10)
                .Select((d, index) => new DistrictRank
                {
                    Rank = index + 1,
                    DistrictName = d.DistrictName,
                    AvgTemperature = d.AvgTemperature,
                    AvgPm25 = d.AvgPm25,
                })
                .ToList();

            return new Top10Response
            {
                GeneratedAt = DateTime.UtcNow,
                Districts = top10
            };
        }

        private async Task<DistrictWeatherSummary> ProcessDistrictAsync(Districts district)
        {
            var startDate = DateTime.Now.ToString("yyyy-MM-dd"); ;
            var endDate = DateTime.Now.AddDays(5).ToString("yyyy-MM-dd");// air quality limitations for 7 days

            // Call both APIs in parallel
            var weatherTask = _weatherService.GetWeatherForecastAsync(district.Latitude, district.Longitude,startDate, endDate);
            var airQualityTask = _airQualityService.GetAirQualityAsync(district.Latitude, district.Longitude, startDate, endDate);

            await Task.WhenAll(weatherTask, airQualityTask);

            var weatherData = await weatherTask;
            var airQualityData = await airQualityTask;

            // Extract 2 PM values and calculate averages
            var temps2PM = ExtractValuesAt2PM(weatherData.Hourly.Temperature2m);
            var pm25_2PM = ExtractValuesAt2PM(airQualityData.Hourly.Pm25);

            return new DistrictWeatherSummary
            {

                DistrictId = district.DivisionId,
                DistrictName = district.Name,
                Latitude = district.Latitude,
                Longitude = district.Longitude,
                AvgTemperature = temps2PM.Any() ? temps2PM.Average() : 0,
                AvgPm25 = pm25_2PM.Any() ? pm25_2PM.Average() : 0
            };
        }

        private List<double> ExtractValuesAt2PM(List<double?> hourlyValues)
        {
            // Hourly data: 7 days × 24 hours = 168 entries
            // Index 14 = Day 1 at 14:00 (2 PM)
            // Index 38 = Day 2 at 14:00 (2 PM) ... and so on

            var values2PM = new List<double>();

            for (int day = 0; day < 7; day++)
            {
                int index = (day * 24) + 14;  // 14 = 2 PM (14:00)

                if (index < hourlyValues.Count && hourlyValues[index].HasValue)
                {
                    values2PM.Add(hourlyValues[index].Value);
                }
            }

            return values2PM;
        }

        //private string GetAirQualityStatus(double pm25)
        //{
        //    // Based on US EPA Air Quality Index for PM2.5
        //    return pm25 switch
        //    {
        //        <= 12 => "Good",
        //        <= 35.4 => "Moderate",
        //        <= 55.4 => "Unhealthy for Sensitive Groups",
        //        <= 150.4 => "Unhealthy",
        //        <= 250.4 => "Very Unhealthy",
        //        _ => "Hazardous"
        //    };
        //}
    }
}

