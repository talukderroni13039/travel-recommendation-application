using TravelRecommendation.Application.DTO.Responses;

namespace TravelRecommendation.Application.Interface
{
    public interface IDistrictService
    {
        Task<Top10Response> GetTop10DistrictsAsync();
    }
}
