using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelRecommendation.Application.DTO;

namespace TravelRecommendation.Application.Services
{
    public interface IWeatherApiClient
    {
        Task<WeatherApiResponse> GetWeatherForecastAsync(double latitude, double longitude);
    }
}
