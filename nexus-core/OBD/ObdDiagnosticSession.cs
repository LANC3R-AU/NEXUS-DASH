using Nexus.J2534.Bridge.J2534;
using Nexus.J2534.Bridge.Protocols;

namespace Nexus.J2534.Bridge.OBD;

public sealed class ObdDiagnosticSession
{
    private readonly J2534Api _api;
    private readonly Iso15765Channel _channel;

    public ObdDiagnosticSession(
        J2534Api api,
        Iso15765Channel channel)
    {
        _api = api;
        _channel = channel;
    }

    public bool IsReady =>
        _channel.IsConnected;

    public byte[] BuildRequest(byte pid)
    {
        EnsureReady();

        return ObdRequest.CreateMode01(pid);
    }

    public unsafe PassThruMessage BuildJ2534Request(
        byte pid)
    {
        EnsureReady();

        byte[] payload =
            BuildRequest(pid);

        return Iso15765MessageBuilder.CreateRequest(
            payload,
            Iso15765MessageBuilder.FunctionalRequestId
        );
    }

    public double DecodeResponse(
        byte expectedPid,
        ReadOnlySpan<byte> payload)
    {
        EnsureReady();

        ObdResponse response =
            ObdResponse.Parse(payload);

        if (response.Pid != expectedPid)
        {
            throw new InvalidOperationException(
                $"Expected PID 0x{expectedPid:X2}, " +
                $"received PID 0x{response.Pid:X2}."
            );
        }

        return response.DecodeValue();
    }

    public unsafe double? RequestPid(
        byte pid,
        uint timeout = 1000)
    {
        EnsureReady();

        if (!_channel.HasResponseFilter)
        {
            throw new InvalidOperationException(
                "ISO15765 response filter is not installed."
            );
        }

        PassThruMessage request =
            BuildJ2534Request(pid);

        _api.WriteMessage(
            _channel.ChannelId!.Value,
            ref request,
            timeout
        );

        DateTime deadline =
            DateTime.UtcNow.AddMilliseconds(timeout);

        while (DateTime.UtcNow < deadline)
        {
            uint remaining =
                (uint)Math.Max(
                    1,
                    (deadline - DateTime.UtcNow)
                        .TotalMilliseconds
                );

            PassThruMessage? received =
                _api.ReadMessage(
                    _channel.ChannelId.Value,
                    remaining
                );

            if (!received.HasValue)
            {
                return null;
            }

            PassThruMessage message =
                received.Value;

            uint canId =
                Iso15765MessageBuilder.ReadCanId(
                    ref message
                );

            // Only accept normal 11-bit
            // OBD-II ECU response IDs.
            if (canId <
                    Iso15765MessageBuilder.ResponseIdMin ||
                canId >
                    Iso15765MessageBuilder.ResponseIdMax)
            {
                continue;
            }

            byte[] payload =
                Iso15765MessageBuilder.ReadPayload(
                    ref message
                );

            // Ignore unrelated messages.
            if (payload.Length < 2)
            {
                continue;
            }

            if (payload[0] !=
                ObdResponse.Mode01Response)
            {
                continue;
            }

            if (payload[1] != pid)
            {
                continue;
            }

            return DecodeResponse(
                pid,
                payload
            );
        }

        return null;
    }

    public double? RequestRpm(
        uint timeout = 1000)
    {
        return RequestPid(
            ObdPid.EngineRpm,
            timeout
        );
    }

    private void EnsureReady()
    {
        if (!_channel.IsConnected)
        {
            throw new InvalidOperationException(
                "ISO15765 channel is not connected."
            );
        }

        if (!_channel.ChannelId.HasValue)
        {
            throw new InvalidOperationException(
                "ISO15765 channel does not have " +
                "a valid J2534 channel ID."
            );
        }
    }
}