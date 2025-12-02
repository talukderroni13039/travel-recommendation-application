
using TravelRecommendation.Application.Interface;
using TravelRecommendation.Application.Services;

namespace TravelRecommendation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
           //  builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();
            // Register DistrictService as Singleton (data loaded once)
            builder.Services.AddSingleton<IDistrictService, DistrictService>();

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

            builder.Services.AddSingleton<IWeatherService, WeatherService>();
            builder.Services.AddSingleton<IAirQualityService, AirQualityService>();
            builder.Services.AddSingleton<IDistrictService, DistrictService>();
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
