// create web-app
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using VehicleRegistrationService.Repositories;

var appName = "VehicleRegistrationService";

var builder = WebApplication.CreateBuilder(args);

var configurationBuilder = new ConfigurationBuilder()
        //.SetBasePath(env.ContentRootPath)
        //.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
        .AddEnvironmentVariables();


var seqServerUrl = configurationBuilder.Build()["SeqServerUrl"];


builder.Host.UseSerilog((ctx, lc) => {
    lc.ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.Seq(seqServerUrl)
    .MinimumLevel.Debug()
    .Enrich.WithProperty("MachineName", Environment.MachineName)
    .Enrich.WithProperty("ApplicationName", appName);
});

builder.Services.AddScoped<IVehicleInfoRepository, InMemoryVehicleInfoRepository>();
builder.Services.AddControllers();

var app = builder.Build();

// configure web-app
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
// configure routing
app.MapControllers();

app.Logger.LogInformation("going to run VehicleRegistrationService");
// let's go!
app.Run("http://localhost:6002");
