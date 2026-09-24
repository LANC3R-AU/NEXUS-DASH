using Nexus.J2534.Bridge.J2534;
using Nexus.J2534.Bridge.OBD;

namespace Nexus.J2534.Bridge.Protocols;

public sealed class Iso15765Channel : IDisposable
{
    private readonly J2534Device _device;
    private readonly J2534Api _api;

    private uint? _channelId;
    private uint? _responseFilterId;
    private bool _disposed;

    public const uint DefaultBaudRate =
        J2534Constants.CAN_500K;

    public bool IsConnected =>
        _channelId.HasValue;

    public bool HasResponseFilter =>
        _responseFilterId.HasValue;

    public uint? ChannelId =>
        _channelId;

    public uint? ResponseFilterId =>
        _responseFilterId;

    public uint BaudRate { get; private set; }

    public Iso15765Channel(
        J2534Device device,
        J2534Api api)
    {
        _device = device;
        _api = api;
    }

    public uint Connect(
        uint baudRate = DefaultBaudRate)
    {
        ThrowIfDisposed();

        if (_channelId.HasValue)
        {
            return _channelId.Value;
        }

        if (!_device.IsOpen)
        {
            throw new InvalidOperationException(
                "The J2534 device must be opened first."
            );
        }

        _channelId =
            _device.Connect(
                J2534Constants.ISO15765,
                J2534Constants.NO_FLAGS,
                baudRate
            );

        BaudRate = baudRate;

        return _channelId.Value;
    }

    public uint InstallResponseFilter()
    {
        ThrowIfDisposed();

        if (!_channelId.HasValue)
        {
            throw new InvalidOperationException(
                "ISO15765 channel must be connected " +
                "before installing a filter."
            );
        }

        if (_responseFilterId.HasValue)
        {
            return _responseFilterId.Value;
        }

        PassThruMessage mask =
            Iso15765FilterBuilder
                .CreateExactMask();

        PassThruMessage pattern =
            Iso15765FilterBuilder
                .CreateEngineResponsePattern();

        PassThruMessage flowControl =
            Iso15765FilterBuilder
                .CreateEngineFlowControl();

        _responseFilterId =
            _api.StartFilter(
                _channelId.Value,
                J2534Constants.FLOW_CONTROL_FILTER,
                ref mask,
                ref pattern,
                flowControl
            );

        return _responseFilterId.Value;
    }

    public byte[] CreateObdRequest(
        byte pid)
    {
        ThrowIfDisposed();

        return ObdRequest.CreateMode01(
            pid
        );
    }

    public double DecodeObdResponse(
        byte expectedPid,
        ReadOnlySpan<byte> payload)
    {
        ThrowIfDisposed();

        ObdResponse response =
            ObdResponse.Parse(payload);

        if (response.Pid != expectedPid)
        {
            throw new InvalidOperationException(
                $"Expected PID 0x{expectedPid:X2}, " +
                $"but received PID 0x{response.Pid:X2}."
            );
        }

        return response.DecodeValue();
    }

    public double DecodeObdResponse(
        ReadOnlySpan<byte> payload)
    {
        ThrowIfDisposed();

        ObdResponse response =
            ObdResponse.Parse(payload);

        return response.DecodeValue();
    }

    public void Disconnect()
    {
        ThrowIfDisposed();

        if (!_channelId.HasValue)
        {
            return;
        }

        _device.Disconnect();

        _channelId = null;
        _responseFilterId = null;
        BaudRate = 0;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(
                nameof(Iso15765Channel)
            );
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        try
        {
            if (_channelId.HasValue)
            {
                _device.Disconnect();

                _channelId = null;
                _responseFilterId = null;
                BaudRate = 0;
            }
        }
        catch
        {
            // Do not allow cleanup errors
            // to hide the original error.
        }

        _disposed = true;
    }
}