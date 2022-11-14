using System.Text;
using System.Text.Json;
using Simulation.Proxies;
using Simulation.Events;
using MQTTnet;
using MQTTnet.Client;
using System.Threading.Tasks;
using System.Threading;

namespace Simulation;

public class Program
{
    public static async Task Main(string[] args)
    {
        int lanes = 3;
        CameraSimulation[] cameras = new CameraSimulation[lanes];
        for (var i = 0; i < lanes; i++)
        {
            int camNumber = i + 1;
            var trafficControlService = await MqttTrafficControlService.CreateAsync(camNumber);

            cameras[i] = new CameraSimulation(camNumber, trafficControlService);
        }
        Parallel.ForEach(cameras, cam => cam.Start());

        await Task.Run(() => Thread.Sleep(Timeout.Infinite));
    }
}