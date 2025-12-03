using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelRecommendation.Domain;

namespace TravelRecommendation.Application.Interface
{
    public interface IDistrictRepository
    {
        Task<DistrictsRoot> LoadDistrictsFromFile();
    }
}
