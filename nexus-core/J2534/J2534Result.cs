namespace Nexus.J2534.Bridge.J2534;

public static class J2534Result
{
    public static bool IsSuccess(
        int result)
    {
        return result ==
            J2534Constants.STATUS_NOERROR;
    }

    public static bool IsTimeout(
        int result)
    {
        return result ==
            J2534Constants.ERR_TIMEOUT;
    }

    public static bool IsBufferEmpty(
        int result)
    {
        return result ==
            J2534Constants.ERR_BUFFER_EMPTY;
    }

    public static bool IsNoData(
        int result)
    {
        return
            IsTimeout(result) ||
            IsBufferEmpty(result);
    }

    public static string GetName(
        int result)
    {
        return result switch
        {
            J2534Constants.STATUS_NOERROR =>
                "STATUS_NOERROR",

            J2534Constants.ERR_NOT_SUPPORTED =>
                "ERR_NOT_SUPPORTED",

            J2534Constants.ERR_INVALID_CHANNEL_ID =>
                "ERR_INVALID_CHANNEL_ID",

            J2534Constants.ERR_INVALID_PROTOCOL_ID =>
                "ERR_INVALID_PROTOCOL_ID",

            J2534Constants.ERR_NULL_PARAMETER =>
                "ERR_NULL_PARAMETER",

            J2534Constants.ERR_INVALID_IOCTL_VALUE =>
                "ERR_INVALID_IOCTL_VALUE",

            J2534Constants.ERR_INVALID_FLAGS =>
                "ERR_INVALID_FLAGS",

            J2534Constants.ERR_FAILED =>
                "ERR_FAILED",

            J2534Constants.ERR_DEVICE_NOT_CONNECTED =>
                "ERR_DEVICE_NOT_CONNECTED",

            J2534Constants.ERR_TIMEOUT =>
                "ERR_TIMEOUT",

            J2534Constants.ERR_INVALID_MSG =>
                "ERR_INVALID_MSG",

            J2534Constants.ERR_INVALID_TIME_INTERVAL =>
                "ERR_INVALID_TIME_INTERVAL",

            J2534Constants.ERR_EXCEEDED_LIMIT =>
                "ERR_EXCEEDED_LIMIT",

            J2534Constants.ERR_INVALID_MSG_ID =>
                "ERR_INVALID_MSG_ID",

            J2534Constants.ERR_DEVICE_IN_USE =>
                "ERR_DEVICE_IN_USE",

            J2534Constants.ERR_INVALID_IOCTL_ID =>
                "ERR_INVALID_IOCTL_ID",

            J2534Constants.ERR_BUFFER_EMPTY =>
                "ERR_BUFFER_EMPTY",

            J2534Constants.ERR_BUFFER_FULL =>
                "ERR_BUFFER_FULL",

            J2534Constants.ERR_BUFFER_OVERFLOW =>
                "ERR_BUFFER_OVERFLOW",

            J2534Constants.ERR_PIN_INVALID =>
                "ERR_PIN_INVALID",

            J2534Constants.ERR_CHANNEL_IN_USE =>
                "ERR_CHANNEL_IN_USE",

            J2534Constants.ERR_MSG_PROTOCOL_ID =>
                "ERR_MSG_PROTOCOL_ID",

            J2534Constants.ERR_INVALID_FILTER_ID =>
                "ERR_INVALID_FILTER_ID",

            J2534Constants.ERR_NO_FLOW_CONTROL =>
                "ERR_NO_FLOW_CONTROL",

            J2534Constants.ERR_NOT_UNIQUE =>
                "ERR_NOT_UNIQUE",

            J2534Constants.ERR_INVALID_BAUDRATE =>
                "ERR_INVALID_BAUDRATE",

            J2534Constants.ERR_INVALID_DEVICE_ID =>
                "ERR_INVALID_DEVICE_ID",

            _ =>
                $"UNKNOWN_J2534_ERROR_0x{result:X2}"
        };
    }

    public static string GetDescription(
        int result)
    {
        return result switch
        {
            J2534Constants.STATUS_NOERROR =>
                "The operation completed successfully.",

            J2534Constants.ERR_DEVICE_NOT_CONNECTED =>
                "The J2534 interface is not connected.",

            J2534Constants.ERR_TIMEOUT =>
                "No message was received before the timeout expired.",

            J2534Constants.ERR_BUFFER_EMPTY =>
                "No messages are currently available.",

            J2534Constants.ERR_BUFFER_FULL =>
                "The J2534 message buffer is full.",

            J2534Constants.ERR_BUFFER_OVERFLOW =>
                "Messages were lost because the receive buffer overflowed.",

            J2534Constants.ERR_INVALID_CHANNEL_ID =>
                "The J2534 channel ID is invalid.",

            J2534Constants.ERR_INVALID_PROTOCOL_ID =>
                "The requested protocol is invalid or unsupported.",

            J2534Constants.ERR_INVALID_BAUDRATE =>
                "The requested communication speed is unsupported.",

            J2534Constants.ERR_NO_FLOW_CONTROL =>
                "No matching ISO15765 flow-control filter is configured.",

            _ =>
                "The J2534 driver reported an error."
        };
    }

    public static void ThrowIfError(
        int result,
        string operation,
        string? driverMessage = null)
    {
        if (IsSuccess(result))
        {
            return;
        }

        throw new J2534Exception(
            result,
            operation,
            driverMessage
        );
    }
}