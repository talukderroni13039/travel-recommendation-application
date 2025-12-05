using TravelRecommendation.Application.DTO.Responses;

namespace TravelRecommendation.Application.Interface.ExternalApis
{
    public interface IAirQualityApiClient
    {
        Task<AirQualityApiResponse> GetAirQualityAsync(double latitude, double longitude, string startDate, string endDate);
    }


}
