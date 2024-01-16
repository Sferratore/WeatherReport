# Overview
The Weather Report project consists of a .NET-based API. The API provides weather information based on place, date, and hour. It also offers statistics on the weather of the month or the year. It retrieves data from WeatherAPI: https://www.weatherapi.com.

# Getting Started
## Prerequisites
.NET Core 3.1 or later
Windows 10/11 or a compatible OS

## Installation
API Setup:
1. Navigate to the API project directory.
2. Remember to **change the key of WeatherAPI in appsettings.json** in order to use a good one to connect to WeatherAPI.
3. Run dotnet build to compile the project.
4. Run dotnet run to start the API (optional, as the cmdlet can also do this).

# Usage
Start the API:
Execute dotnet run within the API project directory.

# API Reference
## Endpoints
### /WeatherApi/GetWeather:
GET: Retrieves weather information.
Parameters:
placeName: Name of the place.
date: Date in YYYY-MM-DD format.
hour: Hour of the day (0-23).
Responses
The API returns JSON data with weather information.

### /WeatherApi/GetMediumMonthWeather:
GET: Retrieves average weather of the month information.
Parameters:
placeName: Name of the place.
date: Date in YYYY-MM format.
Responses
The API returns JSON data with average monthly weather information.

### /WeatherApi/GetMediumYearWeather:
GET: Retrieves average weather of the year information.
Parameters:
placeName: Name of the place.
date: Date in YYYY format.
Responses
The API returns JSON data with average yearly weather information.

# Development
API Development
Built with .NET Core 3.1.
Follows RESTful principles.

