using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelRecommendation.Application.DTO;

namespace TravelRecommendation.Application.Interface
{
    public interface IAirQualityService
    {
        Task<AirQualityApiResponse> GetAirQualityAsync(double latitude, double longitude);
    }


}
