using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelRecommendation.Application.Services;

namespace TravelRecommendation.Application.Interface
{
    public interface IDistrictService
    {
        Task<Top10Response> GetTop10DistrictsAsync();
    }
}
