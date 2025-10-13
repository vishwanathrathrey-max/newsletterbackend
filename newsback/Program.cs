using Microsoft.EntityFrameworkCore;
using newsback.Data;
using newsback.IRepository;
using newsback.IService;
using newsback.Repository;
using newsback.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

// Add EF Core with PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container
builder.Services.AddControllers();

// Dependency Injection for services and repositories
builder.Services.AddScoped<IUrlMetaDataService, UrlMetaDataService>();
builder.Services.AddScoped<IUrlMetaDataRepository, UrlMetaDataRepository>();


// Required for HttpClientFactory
builder.Services.AddHttpClient();

// Required for Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Add OpenAPI services
builder.Services.AddOpenApi();

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger(); // Generates /openapi/v1.json

    // Serve Swagger UI
    app.UseSwaggerUI(c =>
    {
        // Link the JSON to the UI
        c.SwaggerEndpoint("/openapi/v1.json", "Employee API v1");

        // Optional: serve UI at /openapi
        c.RoutePrefix = "openapi";
    });
}

app.UseCors("AllowReactApp");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();