namespace Nexus.J2534.Bridge.J2534;

public sealed class J2534Exception : Exception
{
    public int ErrorCode { get; }

    public string ErrorName { get; }

    public J2534Exception(
        int errorCode,
        string operation,
        string? driverMessage = null)
        : base(BuildMessage(
            errorCode,
            operation,
            driverMessage))
    {
        ErrorCode = errorCode;
        ErrorName =
            J2534Result.GetName(errorCode);
    }

    private static string BuildMessage(
        int errorCode,
        string operation,
        string? driverMessage)
    {
        string name =
            J2534Result.GetName(errorCode);

        string description =
            J2534Result.GetDescription(
                errorCode
            );

        string message =
            $"{operation} failed: " +
            $"{name} (0x{errorCode:X2}). " +
            description;

        if (!string.IsNullOrWhiteSpace(
                driverMessage))
        {
            message +=
                $" Driver: {driverMessage}";
        }

        return message;
    }
}