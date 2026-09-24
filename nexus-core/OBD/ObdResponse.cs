namespace Nexus.J2534.Bridge.OBD;

public sealed record ObdResponse(
    byte Service,
    byte Pid,
    byte[] Data)
{
    public const byte Mode01Response = 0x41;

    public static ObdResponse Parse(
        ReadOnlySpan<byte> payload)
    {
        if (payload.Length < 2)
        {
            throw new ArgumentException(
                "OBD response is too short."
            );
        }

        byte service = payload[0];
        byte pid = payload[1];

        if (service != Mode01Response)
        {
            throw new ArgumentException(
                $"Unexpected OBD service 0x{service:X2}."
            );
        }

        return new ObdResponse(
            service,
            pid,
            payload[2..].ToArray()
        );
    }

    public double DecodeValue()
    {
        return ObdDecoder.Decode(
            Pid,
            Data
        );
    }
}