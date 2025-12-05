using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelRecommendation.Domain;

namespace TravelRecommendation.Application.Interface.Repositories
{
    public interface IDistrictRepository
    {
        List<Districts> GetAllDistricts();
        Districts GetDistrictByCoordinates(double latitude, double longitude);
        Districts GetDistrictByName(string name);
    }
}
