using System.Text;
using System.Text.Json;
// using Simulation.Proxies;
using Simulation.Events;
//using MQTTnet;
//using MQTTnet.Client;
//using MQTTnet.Client.Options;
using System.Threading.Tasks;
using System.Threading;
using Dapr.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Simulation.DITest;
using System;
using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.Extensions.Logging;

namespace Simulation;

public class Program
{
    public static async Task Main(string[] args)
    {
        var appName = "Simulation2";
        var appBuilder = Host.CreateDefaultBuilder(args);

        appBuilder.ConfigureServices((_, services) =>
        {
            services.AddLogging();

            services.AddTransient<ITransientOperation, DefaultOperation>();
            services.AddScoped<IScopedOperation, DefaultOperation>();
            services.AddSingleton<ISingletonOperation, DefaultOperation>();
            services.AddTransient<OperationLogger>();
        });

        string seqServerUrl = string.Empty;
        appBuilder.ConfigureHostConfiguration((configBuilder) =>
        {
            configBuilder.AddEnvironmentVariables();
            var configuration = configBuilder.Build();
            seqServerUrl = configuration["SeqServerUrl"];

        });

        appBuilder.UseSerilog((hostingContext, services, loggerConfiguration) =>
        {
            loggerConfiguration
            .ReadFrom.Configuration(hostingContext.Configuration)
                .WriteTo.Console()
                .WriteTo.Seq(seqServerUrl)
                .Enrich.WithProperty("ApplicationName", appName)
                .MinimumLevel.Debug();
        });

        using IHost host = appBuilder.Build();

        Log.Information("Starting host, SeqServerUrl:{SeqServerUrl}", seqServerUrl);
        

        RunCamerSimulation(host.Services);
        //ExemplifyScoping(host.Services, "Scope 1");
        //ExemplifyScoping(host.Services, "Scope 2");

        await host.RunAsync();
    }

    static void RunCamerSimulation(IServiceProvider services)
    {
        
        int lanes = 3;
        CameraSimulation[] cameras = new CameraSimulation[lanes];
        for (var i = 0; i < lanes; i++)
        {
            int camNumber = i + 1;

            var logger = services.CreateScope().ServiceProvider.GetService<ILoggerFactory>().CreateLogger<CameraSimulation>();
            cameras[i] = new CameraSimulation(logger, camNumber);
        }
        Parallel.ForEach(cameras, cam => cam.Start());
    }

    static void ExemplifyScoping(IServiceProvider services, string scope)
    {
        using IServiceScope serviceScope = services.CreateScope();
        IServiceProvider provider = serviceScope.ServiceProvider;

        OperationLogger logger = provider.GetRequiredService<OperationLogger>();
        logger.LogOperations($"{scope}-Call 1 .GetRequiredService<OperationLogger>()");

        Console.WriteLine("...");

        logger = provider.GetRequiredService<OperationLogger>();
        logger.LogOperations($"{scope}-Call 2 .GetRequiredService<OperationLogger>()");

        Console.WriteLine();
    }
}