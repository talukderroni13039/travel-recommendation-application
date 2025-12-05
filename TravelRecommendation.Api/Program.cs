using TravelRecommendation.Application.Interface;
using TravelRecommendation.Application.Middleware;
using TravelRecommendation.Application.NewFolder;
using TravelRecommendation.Application.Services;
using TravelRecommendation.Infrastructure.ExternalApis;
using TravelRecommendation.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using TravelRecommendation.Application.Interface.Repositories;
using TravelRecommendation.Application.Interface.ExternalApis;
using TravelRecommendation.Application.Interface.Caching;
using TravelRecommendation.Infrastructure.Cacheing;
namespace TravelRecommendation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Services.AddSwaggerGen();

            // In-Memory Caching
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<IInMemoryCache, InMemoryCache>();

            // Register HttpClientFactory for future Weather/AirQuality API calls
            //builder.Services.AddHttpClient("OpenMeteo", client =>
            //{
            //    client.BaseAddress = new Uri("https://api.open-meteo.com/");
            //    client.Timeout = TimeSpan.FromSeconds(30);
            //});

            //builder.Services.AddHttpClient("AirQuality", client =>
            //{
            //    client.BaseAddress = new Uri("https://air-quality-api.open-meteo.com/");
            //    client.Timeout = TimeSpan.FromSeconds(30);
            //});

            builder.Services.AddHttpClient("OpenMeteo", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["OpenMeteoApi:WeatherBaseUrl"]);
                client.Timeout = TimeSpan.FromSeconds(
                    int.Parse(builder.Configuration["OpenMeteoApi:Timeout"]));
            });

            builder.Services.AddHttpClient("AirQuality", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["OpenMeteoApi:AirQualityBaseUrl"]);
                client.Timeout = TimeSpan.FromSeconds(
                    int.Parse(builder.Configuration["OpenMeteoApi:Timeout"]));
            });


            //  DI for services and repositories    
            builder.Services.AddSingleton<IWeatherApiClient, WeatherApiClient>();
            builder.Services.AddSingleton<IAirQualityApiClient, AirQualityApiClient>();
            builder.Services.AddScoped<IDistrictService, DistrictService>();
            builder.Services.AddSingleton<IDistrictRepository, DistrictRepository>();
            builder.Services.AddSingleton<ITravelRecommendationService, TravelRecommendationService>();

            // Fluent Validation
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<TravelRecommendationRequestValidator>();


            var app = builder.Build();

            // initialize cache at startup for response time<500ms from first request
            using (var scope = app.Services.CreateScope())
            {
                var cache = scope.ServiceProvider.GetRequiredService<IInMemoryCache>();
                var districtService = scope.ServiceProvider.GetRequiredService<IDistrictService>();

                var data = await districtService.GetTop10DistrictsAsync();
                await cache.SetAsync("Top10Districts", data);
            }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            // Global Exception Handling Middleware
            app.UseMiddleware<GlobalExceptionHandler>();

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
