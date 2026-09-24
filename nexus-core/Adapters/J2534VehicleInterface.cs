using Nexus.J2534.Bridge.J2534;
using Nexus.J2534.Bridge.OBD;
using Nexus.J2534.Bridge.Protocols;

namespace Nexus.J2534.Bridge.Adapters;

/// <summary>
/// NEXUS vehicle interface implementation for
/// SAE J2534 Pass-Thru devices such as the OpenPort 2.0.
/// </summary>
public sealed class J2534VehicleInterface :
    IVehicleInterface
{
    private readonly J2534Api _api;
    private readonly J2534Device _device;
    private readonly Iso15765Channel _channel;

    private ObdDiagnosticSession? _obdSession;

    private bool _disposed;

    public string Name =>
        "J2534 Pass-Thru";

    /// <summary>
    /// Capabilities currently exposed by the
    /// NEXUS J2534 implementation.
    ///
    /// These describe what NEXUS currently supports,
    /// not every feature that a specific J2534 device
    /// may physically support.
    /// </summary>
    public VehicleInterfaceCapabilities Capabilities { get; } =
        new()
        {
            Obd2 = true,
            Iso15765 = true,
            Can = true,

            CanFd = false,
            Lin = false,
            KLine = false,

            CanChannelCount = 1,

            RawCanAccess = false,
            DiagnosticAccess = true
        };

    public bool IsOpen =>
        _device.IsOpen;

    public bool IsConnected =>
        _channel.IsConnected;

    public J2534VehicleInterface(
        string functionLibrary)
    {
        if (string.IsNullOrWhiteSpace(
                functionLibrary))
        {
            throw new ArgumentException(
                "J2534 function library path " +
                "cannot be empty.",
                nameof(functionLibrary)
            );
        }

        _api =
            new J2534Api(
                functionLibrary
            );

        _device =
            new J2534Device(
                _api
            );

        _channel =
            new Iso15765Channel(
                _device,
                _api
            );
    }

    public void Open()
    {
        ThrowIfDisposed();

        if (_device.IsOpen)
        {
            return;
        }

        _device.Open();
    }

    public void Connect()
    {
        ThrowIfDisposed();

        if (!_device.IsOpen)
        {
            throw new InvalidOperationException(
                "The J2534 device must be opened " +
                "before connecting."
            );
        }

        if (_channel.IsConnected)
        {
            return;
        }

        _channel.Connect();

        _channel.InstallResponseFilter();

        _obdSession =
            new ObdDiagnosticSession(
                _api,
                _channel
            );
    }

    public double? RequestObdPid(
        byte pid,
        uint timeout = 1000)
    {
        ThrowIfDisposed();

        if (!_channel.IsConnected)
        {
            throw new InvalidOperationException(
                "The vehicle interface is not connected."
            );
        }

        if (_obdSession is null)
        {
            throw new InvalidOperationException(
                "The OBD diagnostic session " +
                "has not been initialized."
            );
        }

        return _obdSession.RequestPid(
            pid,
            timeout
        );
    }

    public void Disconnect()
    {
        ThrowIfDisposed();

        _obdSession = null;

        if (_channel.IsConnected)
        {
            _channel.Disconnect();
        }
    }

    public void Close()
    {
        ThrowIfDisposed();

        _obdSession = null;

        if (_channel.IsConnected)
        {
            _channel.Disconnect();
        }

        if (_device.IsOpen)
        {
            _device.Close();
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(
                nameof(J2534VehicleInterface)
            );
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _obdSession = null;

        try
        {
            _channel.Dispose();
        }
        catch
        {
            // Cleanup should not hide
            // an earlier failure.
        }

        try
        {
            _device.Dispose();
        }
        catch
        {
            // Cleanup should not hide
            // an earlier failure.
        }

        _disposed = true;
    }
}