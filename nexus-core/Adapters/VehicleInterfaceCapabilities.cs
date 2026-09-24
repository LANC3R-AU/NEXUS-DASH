namespace Nexus.J2534.Bridge.Adapters;

/// <summary>
/// Describes the communication capabilities exposed
/// by a NEXUS vehicle interface.
/// </summary>
public sealed record VehicleInterfaceCapabilities
{
    public bool Obd2 { get; init; }

    public bool Iso15765 { get; init; }

    public bool Can { get; init; }

    public bool CanFd { get; init; }

    public bool Lin { get; init; }

    public bool KLine { get; init; }

    public int CanChannelCount { get; init; }

    public bool RawCanAccess { get; init; }

    public bool DiagnosticAccess { get; init; }
}