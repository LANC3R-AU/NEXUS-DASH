namespace Nexus.J2534.Bridge.OBD;

public static class ObdPid
{
    // SAE J1979 Mode 01 PIDs

    public const byte CoolantTemperature = 0x05;
    public const byte EngineRpm = 0x0C;
    public const byte VehicleSpeed = 0x0D;
    public const byte FuelLevel = 0x2F;

    public static string GetName(byte pid)
    {
        return pid switch
        {
            CoolantTemperature => "Coolant Temperature",
            EngineRpm => "Engine RPM",
            VehicleSpeed => "Vehicle Speed",
            FuelLevel => "Fuel Level",
            _ => $"Unknown PID 0x{pid:X2}"
        };
    }
}