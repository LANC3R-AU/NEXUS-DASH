using Nexus.J2534.Bridge.OBD;

namespace Nexus.J2534.Bridge.VehicleProfiles;

public sealed class GenericObd2Profile : IVehicleProfile
{
    public string Id =>
        "generic-obd2";

    public string Manufacturer =>
        "Generic";

    public string Model =>
        "OBD-II Vehicle";

    public string? Generation =>
        null;

    public int? YearFrom =>
        null;

    public int? YearTo =>
        null;

    public string Description =>
        "Generic vehicle using standard OBD-II telemetry.";

    public IReadOnlyList<byte> StandardObdPids { get; } =
        new byte[]
        {
            ObdPid.EngineRpm,
            ObdPid.VehicleSpeed,
            ObdPid.CoolantTemperature,
            ObdPid.FuelLevel
        };
}