using Backend.Application.Interface.Caching;
using Microsoft.AspNetCore.Mvc;
using TravelRecommendation.Application.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TravelRecommendation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TravelRecommendationController : ControllerBase
    {
        public readonly IDistrictService _districtService;
        private readonly IInMemoryCache _cacheService;
        public TravelRecommendationController(IInMemoryCache icacheService, ILogger<TravelRecommendationController> logger, IDistrictService districtService)
        {
            _logger = logger;
            _districtService = districtService;
            _cacheService= icacheService;   
        }   
        public readonly ILogger<TravelRecommendationController> _logger;

        [HttpGet("top10")]
        public async Task<IActionResult> GetTop10Districts()
        {
            _logger.LogInformation("Top 10 districts request received");

            var result = await _cacheService.GetOrSetAsync(
                      "Top10Districts",
                       async () => await _districtService.GetTop10DistrictsAsync()
                   );

            _logger.LogInformation("Request completed: GET /api/districts/top10");

            return Ok(result);
        }

        
      
    }
}
