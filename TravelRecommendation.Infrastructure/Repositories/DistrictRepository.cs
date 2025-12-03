using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TravelRecommendation.Application.Interface;
using TravelRecommendation.Domain;

namespace TravelRecommendation.Infrastructure.Repositories
{
    public class DistrictRepository : IDistrictRepository
    {
            private readonly ILogger<DistrictRepository> _logger;
            private readonly List<Districts> _districtsData;

            public DistrictRepository(ILogger<DistrictRepository> logger)
            {
                _logger = logger;

                var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "bd-districts.json");
                _logger.LogInformation("Loading districts from {FilePath}", filePath);

                var json = File.ReadAllText(filePath);
                var result = JsonConvert.DeserializeObject<DistrictsRoot>(json);
                _districtsData = result?.Districts ?? new List<Districts>();

            }

            public List<Districts> GetAllDistricts()
            {
              return _districtsData;
            }
            public Districts GetDistrictByName(string name)
            {
                return  _districtsData
                               .FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }

    }




        //public async Task<DistrictsRoot> LoadDistrictsFromFile()
        //{
        //    try
        //    {
        //        var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "bd-districts.json");

        //        _logger.LogInformation("Loading districts from {FilePath}", filePath);

        //        var json =await File.ReadAllTextAsync(filePath);
        //        var result = JsonConvert.DeserializeObject<DistrictsRoot>(json);

        //        _logger.LogInformation("Loaded {Count} districts", result?.Districts.Count ?? 0);

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error loading districts from file");
        //        throw;
        //    }
        //}


    
}
