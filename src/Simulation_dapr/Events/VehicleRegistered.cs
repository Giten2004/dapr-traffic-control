using System;

namespace Simulation.Events;

public record struct VehicleRegistered(int Lane, string LicenseNumber, DateTime Timestamp);
