namespace Nexus.J2534.Bridge.Models;

public static class BridgeOutput
{
    private static readonly object OutputLock = new();

    public static void Send(BridgeMessage message)
    {
        lock (OutputLock)
        {
            Console.WriteLine(message.ToJson());
        }
    }

    public static void Status(
        string type,
        string status,
        string? message = null)
    {
        Send(new BridgeMessage
        {
            Type = type,
            Status = status,
            Message = message
        });
    }

    public static void Error(
        string error,
        string message)
    {
        Send(new BridgeMessage
        {
            Type = "bridge-error",
            Status = "error",
            Error = error,
            Message = message
        });
    }
}