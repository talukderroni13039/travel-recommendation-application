using TravelRecommendation.Application.DTO.Responses;

namespace TravelRecommendation.Application.Interface.ExternalApis
{
    public interface IWeatherApiClient
    {
        Task<WeatherApiResponse> GetWeatherForecastAsync(double latitude, double longitude, string startDate,string endDate);
    }
}
