namespace Nexus.J2534.Bridge.J2534;

public static class J2534Constants
{
    // Result codes
    public const int STATUS_NOERROR = 0x00;
    public const int ERR_NOT_SUPPORTED = 0x01;
    public const int ERR_INVALID_CHANNEL_ID = 0x02;
    public const int ERR_INVALID_PROTOCOL_ID = 0x03;
    public const int ERR_NULL_PARAMETER = 0x04;
    public const int ERR_INVALID_IOCTL_VALUE = 0x05;
    public const int ERR_INVALID_FLAGS = 0x06;
    public const int ERR_FAILED = 0x07;
    public const int ERR_DEVICE_NOT_CONNECTED = 0x08;
    public const int ERR_TIMEOUT = 0x09;
    public const int ERR_INVALID_MSG = 0x0A;
    public const int ERR_INVALID_TIME_INTERVAL = 0x0B;
    public const int ERR_EXCEEDED_LIMIT = 0x0C;
    public const int ERR_INVALID_MSG_ID = 0x0D;
    public const int ERR_DEVICE_IN_USE = 0x0E;
    public const int ERR_INVALID_IOCTL_ID = 0x0F;
    public const int ERR_BUFFER_EMPTY = 0x10;
    public const int ERR_BUFFER_FULL = 0x11;
    public const int ERR_BUFFER_OVERFLOW = 0x12;
    public const int ERR_PIN_INVALID = 0x13;
    public const int ERR_CHANNEL_IN_USE = 0x14;
    public const int ERR_MSG_PROTOCOL_ID = 0x15;
    public const int ERR_INVALID_FILTER_ID = 0x16;
    public const int ERR_NO_FLOW_CONTROL = 0x17;
    public const int ERR_NOT_UNIQUE = 0x18;
    public const int ERR_INVALID_BAUDRATE = 0x19;
    public const int ERR_INVALID_DEVICE_ID = 0x1A;

    // Protocols
    public const uint J1850VPW = 1;
    public const uint J1850PWM = 2;
    public const uint ISO9141 = 3;
    public const uint ISO14230 = 4;
    public const uint CAN = 5;
    public const uint ISO15765 = 6;

    // CAN speeds
    public const uint CAN_125K = 125000;
    public const uint CAN_250K = 250000;
    public const uint CAN_500K = 500000;
    public const uint CAN_1M = 1000000;

    // Flags
    public const uint NO_FLAGS = 0x00000000;
    public const uint ISO15765_FRAME_PAD = 0x00000040;
    public const uint CAN_29BIT_ID = 0x00000100;

    // Filters
    public const uint PASS_FILTER = 0x00000001;
    public const uint BLOCK_FILTER = 0x00000002;
    public const uint FLOW_CONTROL_FILTER = 0x00000003;
}