Travel Recommendation API
A high-performance REST API built with .NET 9 that provides intelligent travel recommendations for Bangladesh districts based on real-time weather conditions and air quality data."

Features
•	Smart District Ranking - Get top 10 best districts for travel based on temperature and air quality
•	Personalized Recommendations - Compare your current location with destination weather conditions
•	Real-time Data - Fetches live weather and air quality data from Open-Meteo APIs
•	High Performance - Response time < 500ms with intelligent caching
Architecture
This project follows Clean Architecture principles with clear separation of concerns:
TravelRecommendation/
├── TravelRecommendation.Api           # Presentation Layer (Controllers, Middleware)
├── TravelRecommendation.Application   # Business Logic Layer (Services, DTOs, Validators)
├── TravelRecommendation.Domain        # Domain Layer (Entities)
└── TravelRecommendation.Infrastructure # Infrastructure Layer (External APIs, Caching)

Key Technologies:
•	.NET 9 - Latest .NET framework
•	ASP.NET Core Web API - RESTful API framework
•	Open-Meteo APIs - Weather and Air Quality data provider
•	FluentValidation - Request validation
•	In Memory Caching - Response caching for performance
•	Swagger - API documentation



Prerequisites
Before you begin, ensure you have the following installed:


Software	Version	        Download Link
.NET SDK	9.0 orhigher	     Download

Visual Studio	2022 (v17.8+)	 Download

Git	Latest	Download

Visual Studio Workloads Required:
•	ASP.NET and web development
•	
Getting Started
Clone the Repository
bash
git clone https://github.com/talukderroni13039/travel-recommendation-application.git
cd travel-recommendation-application
Open in Visual Studio 2022
Option A: Using Visual Studio
1.	Open Visual Studio 2022
2.	Click File → Open → Project/Solution
3.	Navigate to the cloned folder
4.	Select TravelRecommendation.sln
Option B: Using Command Line
bash
start TravelRecommendation.sln
Restore NuGet Packages
Visual Studio will automatically restore packages. If not:
Method 1: Visual Studio
Right-click on Solution → Restore NuGet Packages
Method 2: Command Line
bash
dotnet restore
Build the Solution
Method 1: Visual Studio
Press Ctrl + Shift + B or click Build → Build Solution
Run the Application
Method 1: Visual Studio (Recommended)
1.	Set TravelRecommendation.Api as the startup project (right-click → Set as Startup Project)
2.	Press F5 (Debug) or Ctrl + F5 (Run without debugging)
Method 2: Command Line
bash
cd TravelRecommendation.Api
dotnet run
The API should now be running at:

- **HTTPS:** `https://localhost:7046`
- **Swagger UI:** `https://localhost:7046/swagger/index.html`

You should see Swagger documentation in your browser automatically.



## API Endpoints
Get Top 10 Districts
Returns the top 10 best districts for travel, ranked by coolest temperature and best air quality.
Endpoint:
https://localhost:7046/api/weather/top10-districts
Sample Response:
{
  "generatedAt": "2025-12-05T18:42:11.2429983Z",
  "districts": [
    {
      "rank": 1,
      "districtName": "Noakhali",
      "avgTemperature": 17.7,
      "avgPm25": 73.5
    },
    {
      "rank": 2,
      "districtName": "Gopalganj",
      "avgTemperature": 17.7,
      "avgPm25": 83.7
    }]}
Response Time: ~15-50ms




Get Travel Recommendation
Compares weather conditions between your current location and destination district.
Endpoint: POST
https://localhost:7046/api/weather/recommendation?latitude=double&longitude=double &destinationDistrict=string&travelDate=date

Query Parameters:
Parameter	Type	Required	Description	Example
latitude	double	Yes	Current location latitude	23.7115253
longitude	double	Yes	Current location longitude	90.4111451
destinationDistrict	string	Yes	Name of destination district	Faridpur
travelDate	string	Yes	Travel date in yyyy-MM-dd format (max 5 days ahead)	2025-12-10
Sample Request: POST
https://localhost:7046/api/weather/recommendation?latitude=23.7115253&longitude=90.4111451&destinationDistrict=Faridpur&travelDate=2025-12-11

Sample Response:
{
    "recommendation": "Not Recommended",
    "reason": "Your destination is 0.4°C hotter and has worse air quality than your current location. It's better to stay where you are.",
    "comparison": {
        "currentLocation": {
            "name": "Dhaka",
            "latitude": 23.7115253,
            "longitude": 90.4111451,
            "date": "2025-12-11T00:00:00",
            "temperatureAt2PM": 19,
            "pm25At2PM": 0
        },
        "destination": {
            "name": "Faridpur",
            "latitude": 23.6070822,
            "longitude": 89.8429406,
            "date": "2025-12-11T00:00:00",
            "temperatureAt2PM": 19.4,
            "pm25At2PM": 0
        }
    }
}
```

Configuration
appsettings.json
json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "OpenMeteoApi": {
    "WeatherBaseUrl": "https://api.open-meteo.com/",
    "AirQualityBaseUrl": "https://air-quality-api.open-meteo.com/",
    "Timeout": 10
  },
  " "CacheSettings": {
   "ExpirationMinutes": 60
 },
}
Testing the API
Using Swagger UI (Recommended)
1.	Navigate to https://localhost:7046/swagger/index.html
2.	Expand the endpoint you want to test
3.	Click Try it out
4.	Fill in the parameters
5.	Click Execute



Using Postman
       Endpoint:
                 https://localhost:7046/api/weather/top10-districts
Endpoint:   POST https://localhost:7046/api/weather/recommendation?latitude=23.7115253&longitude=90.4111451&destinationDistrict=Faridpur&travelDate=2025-12-11
Test each endpoint

Troubleshooting
SSL Certificate Error
Trust the development certificate:
bash
dotnet dev-certs https --trust
NuGet Package Restore Failed
Clear cache and restore:
bash
dotnet nuget locals all --clear
dotnet restore
dotnet build
Port Already in Use
Change the port in launchSettings.json under TravelRecommendation.Api/Properties/
API Returns 400 Bad Request for Valid Date
Date validation restricts travel dates to 5 days ahead due to Open-Meteo Air Quality API limitation.
Note: If today is December 6, 2025, the maximum allowed date is December 11, 2025 (Today + 5 days)
API Validation Rules
Travel Date
•	Format: yyyy-MM-dd (example: 2025-12-10)
•	Range: Today to Today + 5 days
•	Reason: Open-Meteo Air Quality API provides forecasts for only 5 days ahead
Latitude/Longitude
•	Latitude: -90 to 90
•	Longitude: -180 to 180
•	Precision: Up to 7 decimal places
Destination District
•	Must be a valid Bangladesh district name
•	Case-insensitive matching
•	Available districts: 64 districts of Bangladesh
Development Commands
bash
# Clean solution
dotnet clean

# Restore packages
dotnet restore

# Build solution
dotnet build

# Run API
dotnet run --project TravelRecommendation.Api

# Watch mode (auto-reload on changes)
dotnet watch --project TravelRecommendation.Api






Performance Benchmarks
Metric	Value
Top 10 Districts (First Call)	~10ms
Top 10 Districts (Cached)	~5ms

Travel Recommendation (First Call)	~500 ms-1 sec
Travel Recommendation (Cached)	~10-20ms
Contact
Developer: Yunus Ali Rony
GitHub: @talukderroni13039
Project Link: Travel Recommendation API
Acknowledgments
•	Open-Meteo - Free Weather and Air Quality APIs
•	ASP.NET Core - Web framework
•	FluentValidation - Validation library

