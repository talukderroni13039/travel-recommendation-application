using Backend.Application.Interface.Caching;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TravelRecommendation.Application.DTO;
using TravelRecommendation.Application.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TravelRecommendation.Api.Controllers
{
    [Route("api/weather")]
    [ApiController]
    public class TravelRecommendationController : ControllerBase
    {
        public readonly IDistrictService _districtService;
        private readonly IInMemoryCache _cacheService;
        private readonly ITravelRecommendationService _travelRecommendationService;
        public TravelRecommendationController(ITravelRecommendationService travelRecommendationService,  IInMemoryCache icacheService, ILogger<TravelRecommendationController> logger, IDistrictService districtService)
        {
            _logger = logger;
            _districtService = districtService;
            _cacheService= icacheService;
            _travelRecommendationService = travelRecommendationService;
        }   
        public readonly ILogger<TravelRecommendationController> _logger;

        [HttpGet("top10-districts")]
        public async Task<IActionResult> GetTop10Districts()
        {
                _logger.LogInformation("Request started: GET /api/districts/top10");

                var result = await _cacheService.GetOrSetAsync(
                            "Top10Districts",
                            async () => await _districtService.GetTop10DistrictsAsync()
                        );

                _logger.LogInformation("Request completed: GET /api/districts/top10");

                 return Ok(result);
           
        }
        [HttpPost("recommendation")]
        public async Task<IActionResult> GetTravelRecommendation([FromQuery] TravelRecommendationRequest request)
        {
            _logger.LogInformation("Request: POST /api/travel/recommendation");

            var travelDate = DateTime.ParseExact(request.TravelDate,  "yyyy-MM-dd",CultureInfo.InvariantCulture);
            var result = await _travelRecommendationService.GetRecommendationAsync(request.Latitude,request.Longitude,request.DestinationDistrict, travelDate);
            
            _logger.LogInformation("Request completed: POST /api/travel/recommendation");

            return Ok(result);
        }



    }
}
