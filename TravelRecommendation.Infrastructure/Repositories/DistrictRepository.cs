using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TravelRecommendation.Application.Interface;
using TravelRecommendation.Domain;

namespace TravelRecommendation.Infrastructure.Repositories
{
    public class DistrictRepository : IDistrictRepository
    {
        private readonly ILogger<DistrictRepository> _logger;
        public DistrictRepository(ILogger<DistrictRepository> logger)
        {
            _logger = logger;
        }

        public async Task<DistrictsRoot> LoadDistrictsFromFile()
        {
            try
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "bd-districts.json");

                _logger.LogInformation("Loading districts from {FilePath}", filePath);

                var json =await File.ReadAllTextAsync(filePath);
                var result = JsonConvert.DeserializeObject<DistrictsRoot>(json);

                _logger.LogInformation("Loaded {Count} districts", result?.Districts.Count ?? 0);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading districts from file");
                throw;
            }
        }

       
    }
}
