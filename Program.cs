
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.SignalR;
using autofleetapi.Hubs;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllers();

// Add this in the service configuration section (where you add other services)
builder.Services.AddSignalR();



// Get connection string from environment variable
var connectionString = Environment.GetEnvironmentVariable("AUTOFLEET_STRING");

// Check if the connection string is available

if (string.IsNullOrEmpty(connectionString))
{
    // If the environment variable is not found, fallback to the connection string in appsettings.json
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); // Fallback to appsettings.json
    if (string.IsNullOrEmpty(connectionString))
    {
        // Throw an exception if no connection string is found
        throw new InvalidOperationException("Connection string is not set in environment variable or appsettings.json.");
    }
}

// Configure Entity Framework and SQL Server with environment variable
builder.Services.AddDbContext<AutoFleetDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", // Name of the CORS policy
        builder => builder.WithOrigins("http://localhost:3000") // Allow requests from this origin
                          .AllowAnyMethod() // Allow any HTTP method (GET, POST, etc.)
                          .AllowAnyHeader() // Allow any headers
                          .AllowCredentials()); // Allow credentials if needed
});

// Swagger configuration
builder.Services.AddEndpointsApiExplorer(); // Adds API documentation support
builder.Services.AddSwaggerGen(c =>
{
    // Configure Swagger UI for API documentation
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AutoFleet API", Version = "v1" });
});


builder.Services.AddEndpointsApiExplorer();  // Ensure endpoints are properly documented for Swagger
builder.Services.AddSwaggerGen(); // Generate Swagger definitions for API

var app = builder.Build();



// CORS middleware: Applies the CORS policy to incoming requests
app.UseCors("AllowReactApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable Swagger UI in development environment for testing and exploration
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage(); // Show detailed error pages in development
}

app.UseCors(); // Apply CORS middleware globally

app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS

app.UseAuthorization(); // Enable authorization middleware

// Map controllers to routes
app.MapControllers();

// Map SignalR hubs for real-time communication
app.MapHub<NotificationHub>("/notificationHub");



app.Run(); // Run the application
