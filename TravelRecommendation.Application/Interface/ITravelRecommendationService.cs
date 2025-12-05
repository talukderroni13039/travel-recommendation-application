using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelRecommendation.Application.DTO.Responses;

namespace TravelRecommendation.Application.Interface
{
    public interface ITravelRecommendationService
    {
        Task<TravelRecommendationResponse> GetRecommendationAsync(double latitude, double longitude, string destinationDistrict, DateTime travelDate);
    }
}
