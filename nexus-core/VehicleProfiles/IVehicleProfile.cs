namespace Nexus.J2534.Bridge.VehicleProfiles;

public interface IVehicleProfile
{
    string Id { get; }

    string Manufacturer { get; }

    string Model { get; }

    string? Generation { get; }

    int? YearFrom { get; }

    int? YearTo { get; }

    string Description { get; }

    IReadOnlyList<byte> StandardObdPids { get; }
}