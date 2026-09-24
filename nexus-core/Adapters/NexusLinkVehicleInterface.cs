namespace Nexus.J2534.Bridge.Adapters;

/// <summary>
/// Vehicle interface for dedicated NEXUS LINK hardware.
///
/// This is currently a software skeleton.
/// Communication with physical NEXUS LINK hardware
/// will be implemented when the hardware protocol
/// and controller are designed.
/// </summary>
public sealed class NexusLinkVehicleInterface :
    IVehicleInterface
{
    private bool _isOpen;
    private bool _isConnected;
    private bool _disposed;

    public string Name =>
        "NEXUS LINK";

    public VehicleInterfaceCapabilities Capabilities { get; } =
        new()
        {
            Obd2 = true,
            Iso15765 = true,

            Can = true,
            CanFd = true,

            Lin = true,
            KLine = true,

            // Target capability for the future
            // NEXUS LINK hardware.
            CanChannelCount = 2,

            RawCanAccess = true,
            DiagnosticAccess = true
        };

    public bool IsOpen =>
        _isOpen;

    public bool IsConnected =>
        _isConnected;

    public void Open()
    {
        ThrowIfDisposed();

        // Hardware discovery/initialization will
        // eventually happen here.
        _isOpen = true;
    }

    public void Connect()
    {
        ThrowIfDisposed();

        if (!_isOpen)
        {
            throw new InvalidOperationException(
                "NEXUS LINK must be opened " +
                "before connecting."
            );
        }

        // IMPORTANT:
        // Do not pretend that vehicle communication
        // exists before physical hardware support
        // has been implemented.
        throw new NotSupportedException(
            "NEXUS LINK vehicle communication " +
            "has not been implemented yet."
        );
    }

    public double? RequestObdPid(
        byte pid,
        uint timeout = 1000)
    {
        ThrowIfDisposed();

        if (!_isConnected)
        {
            throw new InvalidOperationException(
                "NEXUS LINK is not connected " +
                "to a vehicle network."
            );
        }

        throw new NotSupportedException(
            "NEXUS LINK OBD-II communication " +
            "has not been implemented yet."
        );
    }

    public void Disconnect()
    {
        ThrowIfDisposed();

        _isConnected = false;
    }

    public void Close()
    {
        ThrowIfDisposed();

        _isConnected = false;
        _isOpen = false;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(
                nameof(NexusLinkVehicleInterface)
            );
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _isConnected = false;
        _isOpen = false;

        _disposed = true;
    }
}