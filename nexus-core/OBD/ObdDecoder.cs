namespace Nexus.J2534.Bridge.OBD;

public static class ObdDecoder
{
    public static double DecodeRpm(
        byte a,
        byte b)
    {
        return ((a * 256) + b) / 4.0;
    }

    public static double DecodeSpeed(
        byte a)
    {
        // PID 0D is already km/h.
        return a;
    }

    public static double DecodeCoolant(
        byte a)
    {
        return a - 40;
    }

    public static double DecodeFuelLevel(
        byte a)
    {
        return (a * 100.0) / 255.0;
    }

    public static double Decode(
        byte pid,
        ReadOnlySpan<byte> data)
    {
        return pid switch
        {
            ObdPid.EngineRpm when data.Length >= 2
                => DecodeRpm(data[0], data[1]),

            ObdPid.VehicleSpeed when data.Length >= 1
                => DecodeSpeed(data[0]),

            ObdPid.CoolantTemperature when data.Length >= 1
                => DecodeCoolant(data[0]),

            ObdPid.FuelLevel when data.Length >= 1
                => DecodeFuelLevel(data[0]),

            _ => throw new ArgumentException(
                $"PID 0x{pid:X2} is unsupported or " +
                "does not contain enough data."
            )
        };
    }
}