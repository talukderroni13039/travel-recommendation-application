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
        public TravelRecommendationController(ILogger<TravelRecommendationController> logger, IDistrictService districtService)
        {
            _logger = logger;
            _districtService = districtService;
        }   
        public readonly ILogger<TravelRecommendationController> _logger;

        [HttpGet("top10")]
        public async Task<IActionResult> GetTop10Districts()
        {
            _logger.LogInformation("Top 10 districts request received");

            var result = await _districtService.GetTop10DistrictsAsync();

            _logger.LogInformation("Top 10 districts request completed");

            return Ok(result);
        }

        
      
    }
}
