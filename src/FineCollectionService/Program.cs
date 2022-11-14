// create web-app
using Serilog;

var appName = "FineCollectionService";
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

builder.Services.AddSingleton<IFineCalculator, HardCodedFineCalculator>();



//var daprHttpPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3601";
//var daprGrpcPort = Environment.GetEnvironmentVariable("DAPR_GRPC_PORT") ?? "60001";
//builder.Services.AddDaprClient(builder => builder
//    .UseHttpEndpoint($"http://localhost:{daprHttpPort}")
//    .UseGrpcEndpoint($"http://localhost:{daprGrpcPort}"));
builder.Services.AddDaprClient();

builder.Services.AddSingleton<VehicleRegistrationService>(_ =>
    new VehicleRegistrationService(DaprClient.CreateInvokeHttpClient("vehicleregistrationservice")));

builder.Services.AddControllers().AddDapr();

var app = builder.Build();

// configure web-app
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseCloudEvents();

// configure routing
app.MapControllers();

app.MapSubscribeHandler();

app.Logger.LogInformation("going to run FineCollectionService");

// let's go!
app.Run("http://localhost:6001");
