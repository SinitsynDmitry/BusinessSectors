

using Microsoft.Extensions.Configuration.Json;

internal class Program
{
    /// <summary>
    /// Mains the.
    /// </summary>
    /// <param name="args">The args.</param>
    private static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        var username = builder.AddParameter("db-username", secret: true);
        var password = builder.AddParameter("db-password", secret: true);
      
        var db = builder.AddPostgres("db", username, password)
               .AddDatabase("sectorsDb");

        var sectorsConfigPath = Path.GetFullPath(Path.Combine("..", "config", "sectors-config.json"));

        var api = builder.AddProject<Projects.BusinessSectors_Server>("businesssectors")
                .WithEnvironment("SectorsConfigPath", sectorsConfigPath)   
                .WithReference(db).WaitFor(db);


        builder.AddNpmApp("frontend", "../businesssectors.client", "start")
        .WithReference(api).WaitFor(api)
          .WithEnvironment("API_URL", api.GetEndpoint("https")); // Pass API URL to frontend

        builder.Build().Run();
    }
}