
using Backend.Application.Interface.Caching;
using Backend.Infrastructure.Cacheing.InMemory.Backend.Infrastructure.Cacheing.InMemory;
using TravelRecommendation.Application.Interface;
using TravelRecommendation.Application.Services;
using TravelRecommendation.Infrastructure.ExternalApis;
using TravelRecommendation.Infrastructure.ExternalApiService;
using TravelRecommendation.Infrastructure.Repositories;

namespace TravelRecommendation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Services.AddSwaggerGen();

            // Register HttpClientFactory for future Weather/AirQuality API calls
            builder.Services.AddHttpClient("OpenMeteo", client =>
            {
                client.BaseAddress = new Uri("https://api.open-meteo.com/");
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            builder.Services.AddHttpClient("AirQuality", client =>
            {
                client.BaseAddress = new Uri("https://air-quality-api.open-meteo.com/");
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            builder.Services.AddSingleton<IWeatherApiClient, WeatherApiClient>();
            builder.Services.AddSingleton<IAirQualityApiClient, AirQualityApiClient>();
            builder.Services.AddSingleton<IDistrictService, DistrictService>();
            builder.Services.AddSingleton<IDistrictRepository, DistrictRepository>();
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<IInMemoryCache, InMemoryCache>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
