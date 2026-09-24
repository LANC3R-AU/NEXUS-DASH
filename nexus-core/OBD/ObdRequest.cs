namespace Nexus.J2534.Bridge.OBD;

public static class ObdRequest
{
    public const byte CurrentDataService = 0x01;

    public static byte[] CreateMode01(
        byte pid)
    {
        return new byte[]
        {
            CurrentDataService,
            pid
        };
    }
}