using System.Runtime.InteropServices;

namespace Nexus.J2534.Bridge.J2534;

public sealed class J2534Api : IDisposable
{
    private readonly IntPtr _library;

    private readonly PassThruOpenDelegate _passThruOpen;
    private readonly PassThruCloseDelegate _passThruClose;
    private readonly PassThruConnectDelegate _passThruConnect;
    private readonly PassThruDisconnectDelegate _passThruDisconnect;
    private readonly PassThruReadMsgsDelegate _passThruReadMsgs;
    private readonly PassThruWriteMsgsDelegate _passThruWriteMsgs;
    private readonly PassThruStartMsgFilterDelegate _passThruStartMsgFilter;
    private readonly PassThruGetLastErrorDelegate _passThruGetLastError;

    private bool _disposed;

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int PassThruOpenDelegate(
        IntPtr pName,
        out uint deviceId);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int PassThruCloseDelegate(
        uint deviceId);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int PassThruConnectDelegate(
        uint deviceId,
        uint protocolId,
        uint flags,
        uint baudRate,
        out uint channelId);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int PassThruDisconnectDelegate(
        uint channelId);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private unsafe delegate int PassThruReadMsgsDelegate(
        uint channelId,
        PassThruMessage* message,
        ref uint numMsgs,
        uint timeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private unsafe delegate int PassThruWriteMsgsDelegate(
        uint channelId,
        PassThruMessage* message,
        ref uint numMsgs,
        uint timeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private unsafe delegate int PassThruStartMsgFilterDelegate(
        uint channelId,
        uint filterType,
        PassThruMessage* maskMsg,
        PassThruMessage* patternMsg,
        PassThruMessage* flowControlMsg,
        out uint filterId);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int PassThruGetLastErrorDelegate(
        IntPtr errorDescription);

    public J2534Api(string libraryPath)
    {
        if (!File.Exists(libraryPath))
        {
            throw new FileNotFoundException(
                "J2534 library was not found.",
                libraryPath);
        }

        _library =
            NativeLibrary.Load(libraryPath);

        _passThruOpen =
            LoadFunction<PassThruOpenDelegate>(
                "PassThruOpen");

        _passThruClose =
            LoadFunction<PassThruCloseDelegate>(
                "PassThruClose");

        _passThruConnect =
            LoadFunction<PassThruConnectDelegate>(
                "PassThruConnect");

        _passThruDisconnect =
            LoadFunction<PassThruDisconnectDelegate>(
                "PassThruDisconnect");

        _passThruReadMsgs =
            LoadFunction<PassThruReadMsgsDelegate>(
                "PassThruReadMsgs");

        _passThruWriteMsgs =
            LoadFunction<PassThruWriteMsgsDelegate>(
                "PassThruWriteMsgs");

        _passThruStartMsgFilter =
            LoadFunction<PassThruStartMsgFilterDelegate>(
                "PassThruStartMsgFilter");

        _passThruGetLastError =
            LoadFunction<PassThruGetLastErrorDelegate>(
                "PassThruGetLastError");
    }

    private T LoadFunction<T>(
        string name)
        where T : Delegate
    {
        IntPtr address =
            NativeLibrary.GetExport(
                _library,
                name);

        return Marshal
            .GetDelegateForFunctionPointer<T>(
                address);
    }

    public uint Open()
    {
        int result =
            _passThruOpen(
                IntPtr.Zero,
                out uint deviceId);

        ThrowIfError(
            result,
            "PassThruOpen");

        return deviceId;
    }

    public void Close(
        uint deviceId)
    {
        int result =
            _passThruClose(deviceId);

        ThrowIfError(
            result,
            "PassThruClose");
    }

    public uint Connect(
        uint deviceId,
        uint protocolId,
        uint flags,
        uint baudRate)
    {
        int result =
            _passThruConnect(
                deviceId,
                protocolId,
                flags,
                baudRate,
                out uint channelId);

        ThrowIfError(
            result,
            "PassThruConnect");

        return channelId;
    }

    public void Disconnect(
        uint channelId)
    {
        int result =
            _passThruDisconnect(
                channelId);

        ThrowIfError(
            result,
            "PassThruDisconnect");
    }

    public unsafe void WriteMessage(
        uint channelId,
        ref PassThruMessage message,
        uint timeout = 1000)
    {
        uint count = 1;

        fixed (
            PassThruMessage* ptr =
                &message)
        {
            int result =
                _passThruWriteMsgs(
                    channelId,
                    ptr,
                    ref count,
                    timeout);

            ThrowIfError(
                result,
                "PassThruWriteMsgs");
        }

        if (count != 1)
        {
            throw new InvalidOperationException(
                "J2534 did not transmit " +
                "the requested message.");
        }
    }

    public unsafe PassThruMessage? ReadMessage(
        uint channelId,
        uint timeout = 1000)
    {
        PassThruMessage message =
            default;

        uint count = 1;

        int result =
            _passThruReadMsgs(
                channelId,
                &message,
                ref count,
                timeout);

        // These are normal while polling.
        // They do not mean the bridge failed.
        if (J2534Result.IsNoData(result))
        {
            return null;
        }

        ThrowIfError(
            result,
            "PassThruReadMsgs");

        if (count == 0)
        {
            return null;
        }

        if (count != 1)
        {
            throw new InvalidOperationException(
                "PassThruReadMsgs returned " +
                $"an unexpected message count: {count}."
            );
        }

        return message;
    }

    public unsafe uint StartFilter(
        uint channelId,
        uint filterType,
        ref PassThruMessage mask,
        ref PassThruMessage pattern,
        PassThruMessage? flowControl = null)
    {
        uint filterId;

        fixed (
            PassThruMessage* maskPtr =
                &mask)
        fixed (
            PassThruMessage* patternPtr =
                &pattern)
        {
            if (flowControl.HasValue)
            {
                PassThruMessage flow =
                    flowControl.Value;

                int result =
                    _passThruStartMsgFilter(
                        channelId,
                        filterType,
                        maskPtr,
                        patternPtr,
                        &flow,
                        out filterId);

                ThrowIfError(
                    result,
                    "PassThruStartMsgFilter");
            }
            else
            {
                int result =
                    _passThruStartMsgFilter(
                        channelId,
                        filterType,
                        maskPtr,
                        patternPtr,
                        null,
                        out filterId);

                ThrowIfError(
                    result,
                    "PassThruStartMsgFilter");
            }
        }

        return filterId;
    }

    public string GetLastError()
    {
        IntPtr buffer =
            Marshal.AllocHGlobal(256);

        try
        {
            for (int i = 0; i < 256; i++)
            {
                Marshal.WriteByte(
                    buffer,
                    i,
                    0);
            }

            _passThruGetLastError(
                buffer);

            return Marshal
                       .PtrToStringAnsi(
                           buffer)
                   ?? "Unknown J2534 error";
        }
        finally
        {
            Marshal.FreeHGlobal(
                buffer);
        }
    }

    private void ThrowIfError(
        int result,
        string operation)
    {
        if (J2534Result.IsSuccess(
                result))
        {
            return;
        }

        string? driverMessage = null;

        try
        {
            driverMessage =
                GetLastError();
        }
        catch
        {
            // Preserve the original J2534
            // error if GetLastError fails.
        }

        J2534Result.ThrowIfError(
            result,
            operation,
            driverMessage);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        NativeLibrary.Free(
            _library);

        _disposed = true;
    }
}