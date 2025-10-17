using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using newsback.Configurations;
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

// Configure HttpClientSettings from appsettings.json
builder.Services.Configure<HttpClientSettings>(
    builder.Configuration.GetSection("HttpClientSettings"));

builder.Services.AddHttpClient("OpenGraphClient", (sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<HttpClientSettings>>().Value;
    client.DefaultRequestHeaders.UserAgent.ParseAdd(settings.DefaultUserAgent);
});


// Required for Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Newsletter API",
        Version = "v1",
    });
});


// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Newsletter API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowReactApp");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();