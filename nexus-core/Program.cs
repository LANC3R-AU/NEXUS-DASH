using System.Runtime.InteropServices;
using Nexus.J2534.Bridge.J2534;
using Nexus.J2534.Bridge.Models;
using Nexus.J2534.Bridge.OBD;
using Nexus.J2534.Bridge.Protocols;
using Nexus.J2534.Bridge.VehicleProfiles;

internal static class Program
{
    private const string J2534Library =
        @"C:\Windows\SysWOW64\op20pt32.dll";

    private static int Main(string[] args)
    {
        try
        {
            if (HasArgument(args, "--self-test"))
            {
                return RunSelfTest();
            }

            if (HasArgument(args, "--hardware-test"))
            {
                return RunHardwareTest();
            }

            if (HasArgument(args, "--profiles"))
            {
                return ShowProfiles();
            }

            int vehicleIndex =
                Array.FindIndex(
                    args,
                    arg => string.Equals(
                        arg,
                        "--vehicle",
                        StringComparison.OrdinalIgnoreCase
                    )
                );

            if (vehicleIndex >= 0)
            {
                if (vehicleIndex + 1 >= args.Length)
                {
                    Console.WriteLine(
                        "[NEXUS] Missing vehicle profile."
                    );

                    Console.WriteLine(
                        "Example: --vehicle mitsubishi-lancer-2009"
                    );

                    return 1;
                }

                string profileId =
                    args[vehicleIndex + 1];

                bool readRpm =
                    HasArgument(
                        args,
                        "--read-rpm"
                    );

                return RunVehicle(
                    profileId,
                    readRpm
                );
            }

            ShowHelp();
            return 0;
        }
        catch (Exception ex)
        {
            BridgeOutput.Error(
                ex.GetType().Name,
                ex.Message
            );

            return 1;
        }
    }

    private static bool HasArgument(
        string[] args,
        string argument)
    {
        return args.Contains(
            argument,
            StringComparer.OrdinalIgnoreCase
        );
    }

    private static int RunSelfTest()
    {
        Console.WriteLine(
            "[NEXUS] Running offline self-test..."
        );

        ObdSelfTest.Run();

        Console.WriteLine(
            "[NEXUS] Self-test complete."
        );

        return 0;
    }

    private static int RunHardwareTest()
    {
        BridgeOutput.Send(
            new BridgeMessage
            {
                Type = "bridge-status",
                Status = "starting",
                Source = "nexus-j2534",
                Message =
                    $"Architecture: " +
                    $"{RuntimeInformation.ProcessArchitecture}"
            }
        );

        using var api =
            new J2534Api(J2534Library);

        using var device =
            new J2534Device(api);

        uint deviceId =
            device.Open();

        BridgeOutput.Send(
            new BridgeMessage
            {
                Type = "j2534-status",
                Status = "connected",
                Connected = true,
                Source = "openport2",
                DeviceId = deviceId,
                Message =
                    "J2534 device opened successfully"
            }
        );

        device.Close();

        BridgeOutput.Send(
            new BridgeMessage
            {
                Type = "j2534-status",
                Status = "closed",
                Connected = false,
                Source = "openport2",
                Message =
                    "J2534 device closed successfully"
            }
        );

        return 0;
    }

    private static int RunVehicle(
        string profileId,
        bool readRpm)
    {
        IVehicleProfile? profile =
            VehicleProfileRegistry.GetById(
                profileId
            );

        if (profile is null)
        {
            Console.WriteLine(
                $"[NEXUS] Unknown vehicle profile: " +
                $"{profileId}"
            );

            Console.WriteLine();

            Console.WriteLine(
                "Run --profiles to see available vehicles."
            );

            return 1;
        }

        Console.WriteLine(
            "================================"
        );

        Console.WriteLine(
            " NEXUS VEHICLE MODE"
        );

        Console.WriteLine(
            "================================"
        );

        Console.WriteLine(
            $"Profile:      {profile.Id}"
        );

        Console.WriteLine(
            $"Manufacturer: {profile.Manufacturer}"
        );

        Console.WriteLine(
            $"Model:        {profile.Model}"
        );

        Console.WriteLine();

        Console.WriteLine(
            "[NEXUS] Loading J2534 interface..."
        );

        using var api =
            new J2534Api(J2534Library);

        using var device =
            new J2534Device(api);

        uint deviceId =
            device.Open();

        Console.WriteLine(
            $"[NEXUS] OpenPort connected. " +
            $"Device ID: {deviceId}"
        );

        using var channel =
            new Iso15765Channel(
                device,
                api
            );

        uint channelId =
            channel.Connect();

        Console.WriteLine(
            "[NEXUS] ISO15765 channel ready."
        );

        Console.WriteLine(
            $"[NEXUS] Channel ID: {channelId}"
        );

        Console.WriteLine(
            $"[NEXUS] Baud rate: " +
            $"{channel.BaudRate}"
        );

        Console.WriteLine();

        if (!readRpm)
        {
            Console.WriteLine(
                "[NEXUS] Vehicle profile ready."
            );

            Console.WriteLine(
                "[NEXUS] Diagnostic transmission " +
                "is DISABLED."
            );

            Console.WriteLine();

            Console.WriteLine(
                "No OBD requests were sent."
            );

            return 0;
        }

        return RunRpmTest(
            api,
            channel
        );
    }

    private static int RunRpmTest(
        J2534Api api,
        Iso15765Channel channel)
    {
        Console.WriteLine(
            "[NEXUS] RPM read requested."
        );

        Console.WriteLine(
            "[NEXUS] Installing OBD response filter..."
        );

        uint filterId =
            channel.InstallResponseFilter();

        Console.WriteLine(
            $"[NEXUS] Response filter installed. " +
            $"Filter ID: {filterId}"
        );

        var session =
            new ObdDiagnosticSession(
                api,
                channel
            );

        Console.WriteLine(
            "[NEXUS] Sending Mode 01 PID 0C..."
        );

        double? rpm =
            session.RequestRpm(
                timeout: 2000
            );

        Console.WriteLine();

        if (!rpm.HasValue)
        {
            Console.WriteLine(
                "[NEXUS] No RPM response received."
            );

            return 2;
        }

        Console.WriteLine(
            "================================"
        );

        Console.WriteLine(
            $" ENGINE RPM: {rpm.Value:F0} RPM"
        );

        Console.WriteLine(
            "================================"
        );

        return 0;
    }

    private static int ShowProfiles()
    {
        Console.WriteLine(
            "=============================="
        );

        Console.WriteLine(
            " NEXUS VEHICLE PROFILES"
        );

        Console.WriteLine(
            "=============================="
        );

        foreach (
            IVehicleProfile profile
            in VehicleProfileRegistry.GetAll())
        {
            Console.WriteLine();

            Console.WriteLine(
                $"ID:           {profile.Id}"
            );

            Console.WriteLine(
                $"Manufacturer: {profile.Manufacturer}"
            );

            Console.WriteLine(
                $"Model:        {profile.Model}"
            );

            if (profile.Generation is not null)
            {
                Console.WriteLine(
                    $"Generation:   {profile.Generation}"
                );
            }

            if (profile.YearFrom.HasValue)
            {
                string years =
                    profile.YearTo ==
                    profile.YearFrom
                        ? profile.YearFrom.Value.ToString()
                        : $"{profile.YearFrom}-{profile.YearTo}";

                Console.WriteLine(
                    $"Years:        {years}"
                );
            }

            Console.WriteLine(
                $"OBD PIDs:     " +
                string.Join(
                    ", ",
                    profile.StandardObdPids.Select(
                        pid => $"0x{pid:X2}"
                    )
                )
            );

            Console.WriteLine(
                $"Description:  {profile.Description}"
            );
        }

        Console.WriteLine();
        Console.WriteLine(
            "=============================="
        );

        return 0;
    }

    private static void ShowHelp()
    {
        Console.WriteLine(
            "NEXUS J2534 BRIDGE"
        );

        Console.WriteLine();

        Console.WriteLine("Usage:");

        Console.WriteLine(
            "  --self-test"
        );

        Console.WriteLine(
            "      Run offline tests"
        );

        Console.WriteLine();

        Console.WriteLine(
            "  --hardware-test"
        );

        Console.WriteLine(
            "      Test J2534 interface"
        );

        Console.WriteLine();

        Console.WriteLine(
            "  --profiles"
        );

        Console.WriteLine(
            "      List vehicle profiles"
        );

        Console.WriteLine();

        Console.WriteLine(
            "  --vehicle <profile>"
        );

        Console.WriteLine(
            "      Start NEXUS vehicle mode " +
            "without transmitting OBD requests"
        );

        Console.WriteLine();

        Console.WriteLine(
            "  --vehicle <profile> --read-rpm"
        );

        Console.WriteLine(
            "      Send one read-only Mode 01 " +
            "RPM request"
        );
    }
}