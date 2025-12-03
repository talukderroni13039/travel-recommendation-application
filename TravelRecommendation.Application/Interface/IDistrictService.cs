using TravelRecommendation.Application.DTO;

namespace TravelRecommendation.Application.Interface
{
    public interface IDistrictService
    {
        Task<Top10Response> GetTop10DistrictsAsync();
    }
}
