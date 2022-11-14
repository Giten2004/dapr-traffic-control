// create web-app
using Microsoft.Extensions.Configuration;
using Serilog;

var appName = "TrafficControlService";

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

builder.Services.AddSingleton<ISpeedingViolationCalculator>(
    new DefaultSpeedingViolationCalculator("A12", 10, 100, 5));

builder.Services.AddSingleton<IVehicleStateRepository, DaprVehicleStateRepository>();
builder.Services.AddDaprClient();
builder.Services.AddControllers();
builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<VehicleActor>();
});

var app = builder.Build();

// configure web-app
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCloudEvents();
app.UseRouting();

// configure routing
app.MapControllers();
app.MapActorsHandlers();

// let's go!
app.Run("http://localhost:6000");
