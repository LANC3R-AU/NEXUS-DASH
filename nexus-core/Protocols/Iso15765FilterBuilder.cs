using Nexus.J2534.Bridge.J2534;

namespace Nexus.J2534.Bridge.Protocols;

public static class Iso15765FilterBuilder
{
    public const uint EngineEcuRequestId = 0x7E0;
    public const uint EngineEcuResponseId = 0x7E8;

    public static unsafe PassThruMessage CreateCanIdMessage(
        uint canId)
    {
        PassThruMessage message = default;

        message.ProtocolID =
            J2534Constants.ISO15765;

        message.TxFlags =
            J2534Constants.NO_FLAGS;

        message.DataSize = 4;

        message.Data[0] =
            (byte)((canId >> 24) & 0xFF);

        message.Data[1] =
            (byte)((canId >> 16) & 0xFF);

        message.Data[2] =
            (byte)((canId >> 8) & 0xFF);

        message.Data[3] =
            (byte)(canId & 0xFF);

        return message;
    }

    public static unsafe PassThruMessage CreateExactMask()
    {
        PassThruMessage message = default;

        message.ProtocolID =
            J2534Constants.ISO15765;

        message.DataSize = 4;

        message.Data[0] = 0xFF;
        message.Data[1] = 0xFF;
        message.Data[2] = 0xFF;
        message.Data[3] = 0xFF;

        return message;
    }

    public static PassThruMessage CreateEngineResponsePattern()
    {
        return CreateCanIdMessage(
            EngineEcuResponseId
        );
    }

    public static PassThruMessage CreateEngineFlowControl()
    {
        return CreateCanIdMessage(
            EngineEcuRequestId
        );
    }
}