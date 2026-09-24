namespace Nexus.J2534.Bridge.Adapters;

/// <summary>
/// Common hardware interface used by NEXUS CORE.
///
/// Implementations can communicate through J2534,
/// NEXUS LINK hardware, or future vehicle interfaces.
/// </summary>
public interface IVehicleInterface : IDisposable
{
    /// <summary>
    /// Human-readable adapter name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// True when the physical interface is open.
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    /// True when a vehicle communication channel
    /// has been established.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Opens the physical vehicle interface.
    /// </summary>
    void Open();

    /// <summary>
    /// Establishes the vehicle communication channel.
    /// </summary>
    void Connect();

    /// <summary>
    /// Disconnects from the vehicle network.
    /// </summary>
    void Disconnect();

    /// <summary>
    /// Closes the physical interface.
    /// </summary>
    void Close();

    /// <summary>
    /// Requests a standard OBD-II Mode 01 PID.
    ///
    /// Returns null when the vehicle does not respond
    /// within the timeout.
    /// </summary>
    double? RequestObdPid(
        byte pid,
        uint timeout = 1000
    );
}