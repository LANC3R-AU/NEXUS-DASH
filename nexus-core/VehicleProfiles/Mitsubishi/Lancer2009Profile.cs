using Nexus.J2534.Bridge.OBD;

namespace Nexus.J2534.Bridge.VehicleProfiles.Mitsubishi;

public sealed class Lancer2009Profile : IVehicleProfile
{
    public string Id =>
        "mitsubishi-lancer-2009";

    public string Manufacturer =>
        "Mitsubishi";

    public string Model =>
        "Lancer";

    public string? Generation =>
        "CJ";

    public int? YearFrom =>
        2009;

    public int? YearTo =>
        2009;

    public string Description =>
        "2009 Mitsubishi Lancer CJ vehicle profile.";

    public IReadOnlyList<byte> StandardObdPids { get; } =
        new byte[]
        {
            ObdPid.EngineRpm,
            ObdPid.VehicleSpeed,
            ObdPid.CoolantTemperature,
            ObdPid.FuelLevel
        };
}