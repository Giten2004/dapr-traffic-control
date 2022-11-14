using System.Text;
using System.Text.Json;
//using Simulation.Proxies;
using Simulation.Events;
using Simulation;
//using MQTTnet;
//using MQTTnet.Client;
//using MQTTnet.Client.Options;
using System;
using System.Threading.Tasks;
using Simulation.Proxies;

namespace Simulation;

public class CameraSimulation
{
   private const string PubSub_Name = "mqtt-pubsub";
   private const string Enter_Topic_Name = "trafficcontrol/entrycam";
   private const string Exit_Topic_Name = "trafficcontrol/exitcam";

    private Random _rnd;
    private int _camNumber;
    private int _minEntryDelayInMS = 50;
    private int _maxEntryDelayInMS = 5000;
    private int _minExitDelayInS = 4;
    private int _maxExitDelayInS = 10;

    private ITrafficControlService _trafficControlService;

    public CameraSimulation(int camNumber, ITrafficControlService trafficControlService)
    {
        _rnd = new Random();
        _camNumber = camNumber;
        _trafficControlService = trafficControlService;
    }

    public Task Start()
    {
        Console.WriteLine($"Start camera {_camNumber} simulation.");

        while (true)
        {
            try
            {
                // simulate entry
                TimeSpan entryDelay = TimeSpan.FromMilliseconds(_rnd.Next(_minEntryDelayInMS, _maxEntryDelayInMS) + _rnd.NextDouble());
                Task.Delay(entryDelay).Wait();

                Task.Run(async () =>
                {
                    // simulate entry
                    DateTime entryTimestamp = DateTime.Now;
                    var vehicleRegistered = new VehicleRegistered
                    {
                        Lane = _camNumber,
                        LicenseNumber = GenerateRandomLicenseNumber(),
                        Timestamp = entryTimestamp
                    };
                    //await _trafficControlService.SendVehicleEntryAsync(vehicleRegistered);

                    var eventJson = JsonSerializer.Serialize(vehicleRegistered);
                    await _trafficControlService.SendVehicleEntryAsync(vehicleRegistered);
                    //Console.WriteLine($"enter json: {eventJson}");

                    Console.WriteLine($"Simulated ENTRY of vehicle with license-number {vehicleRegistered.LicenseNumber} in lane {vehicleRegistered.Lane}");

                    var randomNumber = _rnd.Next(1, 20);
                    if (randomNumber == 10)
                    {
                        Console.WriteLine($"Vehicle lost with license-number {vehicleRegistered.LicenseNumber} in lane {vehicleRegistered.Lane}");

                        return;
                    }

                    // simulate exit
                    TimeSpan exitDelay = TimeSpan.FromSeconds(_rnd.Next(_minExitDelayInS, _maxExitDelayInS) + _rnd.NextDouble());
                    await Task.Delay(exitDelay);

                    vehicleRegistered.Timestamp = DateTime.Now;
                    vehicleRegistered.Lane = _rnd.Next(1, 4);
                    //await _trafficControlService.SendVehicleExitAsync(vehicleRegistered);

                    var exiteventJson = JsonSerializer.Serialize(vehicleRegistered);
                    ///Console.WriteLine($"exit json: {exiteventJson}");
                    await _trafficControlService.SendVehicleExitAsync(vehicleRegistered);

                    Console.WriteLine($"Simulated EXIT of vehicle with license-number {vehicleRegistered.LicenseNumber} in lane {vehicleRegistered.Lane}");
                }).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Camera {_camNumber} error: {ex.Message}");
            }
        }
    }

    #region Private helper methods

    private string _validLicenseNumberChars = "DFGHJKLNPRSTXYZ";

    private string GenerateRandomLicenseNumber()
    {
        int type = _rnd.Next(1, 9);
        string kenteken = string.Empty;
        switch (type)
        {
            case 1: // 99-AA-99
                kenteken = string.Format("{0:00}-{1}-{2:00}", _rnd.Next(1, 99), GenerateRandomCharacters(2), _rnd.Next(1, 99));
                break;
            case 2: // AA-99-AA
                kenteken = string.Format("{0}-{1:00}-{2}", GenerateRandomCharacters(2), _rnd.Next(1, 99), GenerateRandomCharacters(2));
                break;
            case 3: // AA-AA-99
                kenteken = string.Format("{0}-{1}-{2:00}", GenerateRandomCharacters(2), GenerateRandomCharacters(2), _rnd.Next(1, 99));
                break;
            case 4: // 99-AA-AA
                kenteken = string.Format("{0:00}-{1}-{2}", _rnd.Next(1, 99), GenerateRandomCharacters(2), GenerateRandomCharacters(2));
                break;
            case 5: // 99-AAA-9
                kenteken = string.Format("{0:00}-{1}-{2}", _rnd.Next(1, 99), GenerateRandomCharacters(3), _rnd.Next(1, 10));
                break;
            case 6: // 9-AAA-99
                kenteken = string.Format("{0}-{1}-{2:00}", _rnd.Next(1, 9), GenerateRandomCharacters(3), _rnd.Next(1, 10));
                break;
            case 7: // AA-999-A
                kenteken = string.Format("{0}-{1:000}-{2}", GenerateRandomCharacters(2), _rnd.Next(1, 999), GenerateRandomCharacters(1));
                break;
            case 8: // A-999-AA
                kenteken = string.Format("{0}-{1:000}-{2}", GenerateRandomCharacters(1), _rnd.Next(1, 999), GenerateRandomCharacters(2));
                break;
        }

        return kenteken;
    }

    private string GenerateRandomCharacters(int aantal)
    {
        char[] chars = new char[aantal];
        for (int i = 0; i < aantal; i++)
        {
            chars[i] = _validLicenseNumberChars[_rnd.Next(_validLicenseNumberChars.Length - 1)];
        }
        return new string(chars);
    }

    #endregion
}
