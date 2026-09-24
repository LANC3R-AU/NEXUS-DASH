using Nexus.J2534.Bridge.J2534;

namespace Nexus.J2534.Bridge.Protocols;

public static class Iso15765MessageBuilder
{
    // Functional OBD-II request address
    public const uint FunctionalRequestId = 0x7DF;

    // Typical ECU response range
    public const uint ResponseIdMin = 0x7E8;
    public const uint ResponseIdMax = 0x7EF;

    public static unsafe PassThruMessage CreateRequest(
        ReadOnlySpan<byte> obdPayload,
        uint canId = FunctionalRequestId)
    {
        if (obdPayload.Length == 0)
        {
            throw new ArgumentException(
                "OBD payload cannot be empty."
            );
        }

        PassThruMessage message = default;

        message.ProtocolID =
            J2534Constants.ISO15765;

        message.TxFlags =
            J2534Constants.ISO15765_FRAME_PAD;

        // J2534 ISO15765 messages begin with
        // the four-byte CAN identifier.
        message.Data[0] =
            (byte)((canId >> 24) & 0xFF);

        message.Data[1] =
            (byte)((canId >> 16) & 0xFF);

        message.Data[2] =
            (byte)((canId >> 8) & 0xFF);

        message.Data[3] =
            (byte)(canId & 0xFF);

        for (int i = 0; i < obdPayload.Length; i++)
        {
            message.Data[4 + i] =
                obdPayload[i];
        }

        message.DataSize =
            (uint)(4 + obdPayload.Length);

        return message;
    }

    public static unsafe uint ReadCanId(
        ref PassThruMessage message)
    {
        if (message.DataSize < 4)
        {
            throw new ArgumentException(
                "J2534 message does not contain a CAN ID."
            );
        }

        return
            ((uint)message.Data[0] << 24) |
            ((uint)message.Data[1] << 16) |
            ((uint)message.Data[2] << 8) |
            message.Data[3];
    }

    public static unsafe byte[] ReadPayload(
        ref PassThruMessage message)
    {
        if (message.DataSize < 4)
        {
            throw new ArgumentException(
                "J2534 message does not contain a valid ISO15765 payload."
            );
        }

        int payloadLength =
            checked((int)message.DataSize - 4);

        byte[] payload =
            new byte[payloadLength];

        for (int i = 0; i < payloadLength; i++)
        {
            payload[i] =
                message.Data[4 + i];
        }

        return payload;
    }
}