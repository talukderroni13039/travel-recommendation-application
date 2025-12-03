using Microsoft.Extensions.Logging;
using TravelRecommendation.Application.DTO;
using TravelRecommendation.Application.Interface;

namespace TravelRecommendation.Application.Services
{
    public class TravelRecommendationService : ITravelRecommendationService
    {
        private readonly IDistrictRepository _districtRepository;
        private readonly IWeatherApiClient _weatherApiClient;
        private readonly IAirQualityApiClient _airQualityApiClient;
        private readonly ILogger<TravelRecommendationService> _logger;

        private const int ForecastDays = 7;
        private const int Hour2PM = 14;

        public TravelRecommendationService(  IDistrictRepository districtRepository, IWeatherApiClient weatherApiClient, IAirQualityApiClient airQualityApiClient, ILogger<TravelRecommendationService> logger)
        {
            _districtRepository = districtRepository;
            _weatherApiClient = weatherApiClient;
            _airQualityApiClient = airQualityApiClient;
            _logger = logger;
        }

        public async Task<TravelRecommendationResponse> GetRecommendationAsync(double latitude, double longitude, string destinationDistrict, DateTime travelDate)
        {
            // Step 2: Get destination district
            var currentDistrict = _districtRepository.GetDistrictByName(destinationDistrict);

            var currentLocationTask = FetchWeatherForLocationAsync(currentDistrict.Latitude, currentDistrict.Longitude, currentDistrict.Name, travelDate);
            var destinationTask = FetchWeatherForLocationAsync(latitude, longitude, destinationDistrict, travelDate);

            await Task.WhenAll(currentLocationTask, destinationTask);

            var currentLocationWeather = await currentLocationTask;
            var destinationWeather = await destinationTask;

            // Step 5: Generate recommendation
            return GenerateRecommendation(currentLocationWeather, destinationWeather);
        }

        private async Task<LocationWeatherInfo?> FetchWeatherForLocationAsync(double latitude, double longitude, string locationName, DateTime travelDate)
        {
            try
            {
                var weatherTask = _weatherApiClient.GetWeatherForecastAsync(latitude, longitude);
                var airQualityTask = _airQualityApiClient.GetAirQualityAsync(latitude, longitude);

                await Task.WhenAll(weatherTask, airQualityTask);

                var weatherResult = await weatherTask;
                var airQualityResult = await airQualityTask;

                var dayOffset = (travelDate.Date - DateTime.UtcNow.Date).Days;
                int index = (dayOffset * 24) + Hour2PM;


                var temperature = weatherResult.Hourly.Temperature2m[index];
                var pm25 = airQualityResult.Hourly.Pm25[index];

                if (!temperature.HasValue || !pm25.HasValue)
                {
                    _logger.LogWarning("Null values at index for: {Name}", locationName);
                    return null;
                }

                return new LocationWeatherInfo
                {
                    Name = locationName,
                    Latitude = latitude,
                    Longitude = longitude,
                    Date = travelDate.Date,
                    TemperatureAt2PM = Math.Round(temperature.Value, 1),
                    Pm25At2PM = Math.Round(pm25.Value, 1),

                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching weather for: {Name}", locationName);
                return null;
            }
        }

        private TravelRecommendationResponse GenerateRecommendation(LocationWeatherInfo current, LocationWeatherInfo destination)
        {
            double tempDiff = current.TemperatureAt2PM - destination.TemperatureAt2PM;
            double pm25Diff = current.Pm25At2PM - destination.Pm25At2PM;

            bool isCooler = destination.TemperatureAt2PM < current.TemperatureAt2PM;
            bool isBetterAir = destination.Pm25At2PM < current.Pm25At2PM;

            string recommendation;
            string reason;

            if (isCooler && isBetterAir)
            {
                // Both better
                recommendation = "Recommended";
                reason = $"Your destination is {Math.Abs(tempDiff):F1}°C cooler and has significantly better air quality. Enjoy your trip!";
            }
            else if (!isCooler && !isBetterAir)
            {
                // Both worse
                recommendation = "Not Recommended";
                reason = $"Your destination is {Math.Abs(tempDiff):F1}°C hotter and has worse air quality than your current location. It's better to stay where you are.";
            }
            else if (isCooler && !isBetterAir)
            {
                // Cooler but worse air
                recommendation = "Partially Recommended";
                reason = $"Your destination is {Math.Abs(tempDiff):F1}°C cooler, but air quality is worse (PM2.5: {destination.Pm25At2PM} vs {current.Pm25At2PM}).";
            }
            else
            {
                // Hotter but better air
                recommendation = "Partially Recommended";
                reason = $"Your destination has better air quality, but is {Math.Abs(tempDiff):F1}°C hotter than your current location.";
            }

            return new TravelRecommendationResponse
            {
                Recommendation = recommendation,
                Reason = reason,
                Comparison = new LocationComparison
                {
                    CurrentLocation = current,
                    Destination = destination
                }
            };
        }



    }
}
