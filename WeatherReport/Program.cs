// Initializes a new instance of the WebApplication builder with preconfigured defaults.
using WeatherReport;

var builder = WebApplication.CreateBuilder(args);

// Add services to the DI (Dependency Injection) container.

//Adds possibility to do Http calls.
builder.Services.AddHttpClient(); 

// Adds MVC controllers to the services collection. 
builder.Services.AddControllers();

//Defining DI for class using appsettings.json.
builder.Services.Configure<WeatherApiSettings>(builder.Configuration.GetSection("WeatherAPI"));

// Adds API Explorer services which are necessary for generating Swagger (OpenAPI) documentation.
// Swagger provides a UI to test your API endpoints and understand the request/response format.
builder.Services.AddEndpointsApiExplorer();

// Adds Swagger generator services, which will produce the Swagger JSON document and UI.
builder.Services.AddSwaggerGen();

// Builds the web application based on the configured services and application builder.
var app = builder.Build();

// Configure the HTTP request pipeline - this is where you configure middleware.

// Checks if the environment is Development. If true, Swagger UI and documentation are enabled.
// This is usually enabled in development to help with API testing and documentation.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();      // Enables Swagger JSON endpoint.
    app.UseSwaggerUI();    // Enables Swagger UI endpoint.
}

// Adds middleware to redirect HTTP requests to HTTPS.
app.UseHttpsRedirection();

// Adds middleware for authorization. This is used in conjunction with Authentication to secure app.
app.UseAuthorization();

// Maps controller routes. This is required for routing to controller actions to work.
app.MapControllers();

// Runs the application. This starts the web server and begins listening for incoming requests.
app.Run();
