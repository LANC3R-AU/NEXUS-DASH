namespace Nexus.J2534.Bridge.J2534;

public sealed class J2534Device : IDisposable
{
    private readonly J2534Api _api;

    private uint? _deviceId;
    private uint? _channelId;

    private bool _disposed;

    public bool IsOpen =>
        _deviceId.HasValue;

    public bool IsChannelConnected =>
        _channelId.HasValue;

    public uint? DeviceId =>
        _deviceId;

    public uint? ChannelId =>
        _channelId;

    public J2534Device(J2534Api api)
    {
        _api = api;
    }

    public uint Open()
    {
        ThrowIfDisposed();

        if (_deviceId.HasValue)
        {
            return _deviceId.Value;
        }

        _deviceId = _api.Open();

        return _deviceId.Value;
    }

    public uint Connect(
        uint protocolId,
        uint flags,
        uint baudRate)
    {
        ThrowIfDisposed();

        if (!_deviceId.HasValue)
        {
            throw new InvalidOperationException(
                "The J2534 device must be opened before connecting a channel."
            );
        }

        if (_channelId.HasValue)
        {
            throw new InvalidOperationException(
                "A J2534 channel is already connected."
            );
        }

        _channelId = _api.Connect(
            _deviceId.Value,
            protocolId,
            flags,
            baudRate
        );

        return _channelId.Value;
    }

    public void Disconnect()
    {
        ThrowIfDisposed();

        if (!_channelId.HasValue)
        {
            return;
        }

        uint channelId =
            _channelId.Value;

        _channelId = null;

        _api.Disconnect(channelId);
    }

    public void Close()
    {
        ThrowIfDisposed();

        if (_channelId.HasValue)
        {
            Disconnect();
        }

        if (!_deviceId.HasValue)
        {
            return;
        }

        uint deviceId =
            _deviceId.Value;

        _deviceId = null;

        _api.Close(deviceId);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(
                nameof(J2534Device)
            );
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        // Best-effort cleanup.
        // Shutdown should continue even if the
        // J2534 driver reports an error.
        try
        {
            if (_channelId.HasValue)
            {
                uint channelId =
                    _channelId.Value;

                _channelId = null;

                _api.Disconnect(channelId);
            }
        }
        catch
        {
        }

        try
        {
            if (_deviceId.HasValue)
            {
                uint deviceId =
                    _deviceId.Value;

                _deviceId = null;

                _api.Close(deviceId);
            }
        }
        catch
        {
        }

        _disposed = true;
    }
}