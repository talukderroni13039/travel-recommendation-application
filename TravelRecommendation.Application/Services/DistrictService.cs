using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using TravelRecommendation.Application.Interface;
using TravelRecommendation.Domain;

namespace TravelRecommendation.Application.Services
{
    public class DistrictService : IDistrictService
    {
        private readonly ILogger<DistrictService> _logger;
        private readonly IWeatherService _weatherService;
        private readonly IAirQualityService _airQualityService;
        private readonly DistrictsRoot _districts;

        public DistrictService(  ILogger<DistrictService> logger, IWeatherService weatherService,  IAirQualityService airQualityService )
        {
            _logger = logger;
             _weatherService = weatherService;
            _airQualityService = airQualityService;
            _districts = LoadDistrictsFromFile();
        }


        public async Task<Top10Response> GetTop10DistrictsAsync()
        {
            _logger.LogInformation("Starting to calculate Top 10 districts...");

            var districtsWeather = new ConcurrentBag<DistrictWeatherSummary>();

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = 10
            };

            await Parallel.ForEachAsync(_districts.Districts, options, async (district, token) =>
            {
                DistrictWeatherSummary districtWeather = await ProcessDistrictAsync(district);
                if (districtWeather != null)
                {
                    districtsWeather.Add(districtWeather);
                }
            });


            _logger.LogInformation("Successfully processed {Count} districts", districtsWeather.Count);

            // Sort by temperature (ascending), then by PM2.5 (ascending) for ties
            var top10 = districtsWeather  // need to change here
                .OrderBy(d => d.AvgTemperature)
                .ThenBy(d => d.AvgPm25)
                .Take(10)
                .Select((d, index) => new DistrictRank
                {
                    Rank = index + 1,
                    DistrictName = d.DistrictName,
                    AvgTemperature = Math.Round(d.AvgTemperature, 1),
                    AvgPm25 = Math.Round(d.AvgPm25, 1),
                    AirQualityStatus = GetAirQualityStatus(d.AvgPm25)
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
            // Call both APIs in parallel
            var weatherTask = _weatherService.GetWeatherForecastAsync(district.Latitude, district.Longitude);
            var airQualityTask = _airQualityService.GetAirQualityAsync(district.Latitude, district.Longitude);

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

        private string GetAirQualityStatus(double pm25)
        {
            // Based on US EPA Air Quality Index for PM2.5
            return pm25 switch
            {
                <= 12 => "Good",
                <= 35.4 => "Moderate",
                <= 55.4 => "Unhealthy for Sensitive Groups",
                <= 150.4 => "Unhealthy",
                <= 250.4 => "Very Unhealthy",
                _ => "Hazardous"
            };
        }

        private DistrictsRoot LoadDistrictsFromFile()
        {
            try
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "bd-districts.json");

                _logger.LogInformation("Loading districts from {FilePath}", filePath);

                var json = File.ReadAllText(filePath);

            

              var result=  JsonConvert.DeserializeObject<DistrictsRoot>(json);

                _logger.LogInformation("Loaded {Count} districts", result?.Districts.Count ?? 0);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading districts from file");
                throw;
            }
        }
    }
    public class Top10Response
    {
        public DateTime GeneratedAt { get; set; }
        public List<DistrictRank> Districts { get; set; }
    }
    public class DistrictRank
    {
        public int Rank { get; set; }
        public string DistrictName { get; set; }
        public double AvgTemperature { get; set; }
        public double AvgPm25 { get; set; }
        public string AirQualityStatus { get; set; }
    }
    public class DistrictWeatherSummary
    {
        public string DistrictId { get; set; }
        public string DistrictName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double AvgTemperature { get; set; }
        public double AvgPm25 { get; set; }
    }


}
