using BusinessSectors.Server.Data;
using BusinessSectors.Server.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

string configDirPath = Environment.GetEnvironmentVariable("SectorsConfigPath");
if (!string.IsNullOrEmpty(configDirPath) && File.Exists(configDirPath))
{
    builder.Configuration.AddJsonFile(configDirPath, optional: true);
}

// Add services to the container.
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
//var connectionDB = builder.Configuration.GetConnectionString("DBConnection") ?? throw new InvalidOperationException($"Connection string DBConnection not found.");
//builder.Services.AddDbContext<SectorsDbContext>(options => options.UseNpgsql(connectionDB));

builder.AddServiceDefaults();

// Add database context with the connection string from Aspire
builder.Services.AddDbContext<SectorsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("sectorsDb")));

// Add CORS service with the configured origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();

builder.Services.AddScoped<ISectorsRepository, SectorsRepository>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("CorsPolicy");

app.UseDefaultFiles();
app.MapStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = scope.ServiceProvider.GetRequiredService<SectorsDbContext>();
   
    try
    {
        dbContext.Database.Migrate();
        var config = services.GetRequiredService<IConfiguration>();
        SeedData.Initialize(dbContext, config);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

await app.RunAsync();
