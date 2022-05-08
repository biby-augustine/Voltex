using API;
using API.Infrastructure.Configurations;
using Serilog;

public class Program
{
    public static async Task Main(string[] args)
    {

        try
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            Log.Logger = SerilogConfigurator.CreateLogger();

            Log.Logger.Information("Starting up");
            Log.Logger.Information("Path : {CurrentDirectory}", Directory.GetCurrentDirectory());

            var config = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("hosting.json", optional: true)
          .Build();

            var urls = config.GetValue<string>("server.urls");

            Log.Information("Url : {Urls}", urls);

            using var webHost = CreateHostBuilder(args, urls).Build();
            await webHost.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Logger.Fatal(ex, "Application start-up failed");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args, string urls) =>
        Host.CreateDefaultBuilder(args)
        .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                if (!string.IsNullOrEmpty(urls))
                {
                    webBuilder.UseUrls(urls.Split(';'));
                }
            });
}
