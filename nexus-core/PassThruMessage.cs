using System.Runtime.InteropServices;

namespace Nexus.J2534.Bridge.J2534;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct PassThruMessage
{
    public uint ProtocolID;
    public uint RxStatus;
    public uint TxFlags;
    public uint Timestamp;
    public uint DataSize;
    public uint ExtraDataIndex;

    public fixed byte Data[4128];
}