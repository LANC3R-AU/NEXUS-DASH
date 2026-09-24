using Nexus.J2534.Bridge.J2534;
using Nexus.J2534.Bridge.Protocols;

namespace Nexus.J2534.Bridge.OBD;

public static class ObdSelfTest
{
    public static void Run()
    {
        Console.WriteLine("================================");
        Console.WriteLine(" NEXUS OBD-II SELF TEST");
        Console.WriteLine("================================");

        TestRpm();
        TestSpeed();
        TestCoolant();
        TestFuel();
        TestIso15765Framing();
        TestIso15765Response();
        TestJ2534Results();
        TestIso15765Filter();

        Console.WriteLine("--------------------------------");
        Console.WriteLine(" ALL OBD TESTS PASSED");
        Console.WriteLine("--------------------------------");
    }

    private static void TestRpm()
    {
        byte[] response =
        {
            0x41,
            0x0C,
            0x1A,
            0xF8
        };

        ObdResponse parsed =
            ObdResponse.Parse(response);

        double value =
            parsed.DecodeValue();

        AssertEqual(
            "Engine RPM",
            1726.0,
            value
        );
    }

    private static void TestSpeed()
    {
        byte[] response =
        {
            0x41,
            0x0D,
            0x64
        };

        ObdResponse parsed =
            ObdResponse.Parse(response);

        double value =
            parsed.DecodeValue();

        AssertEqual(
            "Vehicle Speed",
            100.0,
            value
        );
    }

    private static void TestCoolant()
    {
        byte[] response =
        {
            0x41,
            0x05,
            0x7D
        };

        ObdResponse parsed =
            ObdResponse.Parse(response);

        double value =
            parsed.DecodeValue();

        AssertEqual(
            "Coolant Temperature",
            85.0,
            value
        );
    }

    private static void TestFuel()
    {
        byte[] response =
        {
            0x41,
            0x2F,
            0x80
        };

        ObdResponse parsed =
            ObdResponse.Parse(response);

        double value =
            parsed.DecodeValue();

        double expected =
            (0x80 * 100.0) / 255.0;

        AssertEqual(
            "Fuel Level",
            expected,
            value
        );
    }

    private static unsafe void TestIso15765Framing()
    {
        byte[] obdRequest =
            ObdRequest.CreateMode01(
                ObdPid.EngineRpm
            );

        PassThruMessage message =
            Iso15765MessageBuilder.CreateRequest(
                obdRequest
            );

        uint canId =
            Iso15765MessageBuilder.ReadCanId(
                ref message
            );

        byte[] payload =
            Iso15765MessageBuilder.ReadPayload(
                ref message
            );

        if (canId !=
            Iso15765MessageBuilder.FunctionalRequestId)
        {
            throw new Exception(
                $"CAN ID test failed. " +
                $"Expected 0x7DF, got 0x{canId:X3}."
            );
        }

        if (payload.Length != 2)
        {
            throw new Exception(
                $"OBD payload length failed. " +
                $"Expected 2, got {payload.Length}."
            );
        }

        if (payload[0] != 0x01 ||
            payload[1] != ObdPid.EngineRpm)
        {
            throw new Exception(
                $"OBD payload failed. " +
                $"Expected 01 0C, got " +
                $"{payload[0]:X2} {payload[1]:X2}."
            );
        }

        Console.WriteLine(
            "[PASS] ISO15765 framing: " +
            "7DF -> 01 0C"
        );
    }

    private static unsafe void TestIso15765Response()
    {
        PassThruMessage message =
            Iso15765MessageBuilder.CreateRequest(
                new byte[]
                {
                    0x41,
                    0x0C,
                    0x1A,
                    0xF8
                },
                0x7E8
            );

        uint canId =
            Iso15765MessageBuilder.ReadCanId(
                ref message
            );

        if (canId != 0x7E8)
        {
            throw new Exception(
                $"ECU response CAN ID failed. " +
                $"Expected 0x7E8, got 0x{canId:X3}."
            );
        }

        byte[] payload =
            Iso15765MessageBuilder.ReadPayload(
                ref message
            );

        ObdResponse response =
            ObdResponse.Parse(payload);

        if (response.Pid !=
            ObdPid.EngineRpm)
        {
            throw new Exception(
                $"Expected RPM PID 0x0C, " +
                $"got 0x{response.Pid:X2}."
            );
        }

        double rpm =
            response.DecodeValue();

        AssertEqual(
            "ISO15765 ECU -> RPM",
            1726.0,
            rpm
        );

        Console.WriteLine(
            "[PASS] ECU response: " +
            "7E8 -> 41 0C 1A F8 -> 1726 RPM"
        );
    }

    private static void TestJ2534Results()
    {
        if (!J2534Result.IsSuccess(
                J2534Constants.STATUS_NOERROR))
        {
            throw new Exception(
                "J2534 success result test failed."
            );
        }

        if (!J2534Result.IsNoData(
                J2534Constants.ERR_TIMEOUT))
        {
            throw new Exception(
                "J2534 timeout handling test failed."
            );
        }

        if (!J2534Result.IsNoData(
                J2534Constants.ERR_BUFFER_EMPTY))
        {
            throw new Exception(
                "J2534 buffer-empty handling test failed."
            );
        }

        string timeoutName =
            J2534Result.GetName(
                J2534Constants.ERR_TIMEOUT
            );

        if (timeoutName != "ERR_TIMEOUT")
        {
            throw new Exception(
                "J2534 error-name test failed."
            );
        }

        Console.WriteLine(
            "[PASS] J2534 result/error handling"
        );
    }

    private static unsafe void TestIso15765Filter()
    {
        PassThruMessage mask =
            Iso15765FilterBuilder
                .CreateExactMask();

        PassThruMessage pattern =
            Iso15765FilterBuilder
                .CreateEngineResponsePattern();

        PassThruMessage flowControl =
            Iso15765FilterBuilder
                .CreateEngineFlowControl();

        uint patternId =
            Iso15765MessageBuilder.ReadCanId(
                ref pattern
            );

        uint flowControlId =
            Iso15765MessageBuilder.ReadCanId(
                ref flowControl
            );

        if (patternId !=
            Iso15765FilterBuilder.EngineEcuResponseId)
        {
            throw new Exception(
                $"Expected response ID 0x7E8, " +
                $"got 0x{patternId:X3}."
            );
        }

        if (flowControlId !=
            Iso15765FilterBuilder.EngineEcuRequestId)
        {
            throw new Exception(
                $"Expected flow-control ID 0x7E0, " +
                $"got 0x{flowControlId:X3}."
            );
        }

        if (mask.DataSize != 4 ||
            mask.Data[0] != 0xFF ||
            mask.Data[1] != 0xFF ||
            mask.Data[2] != 0xFF ||
            mask.Data[3] != 0xFF)
        {
            throw new Exception(
                "ISO15765 exact mask test failed."
            );
        }

        Console.WriteLine(
            "[PASS] ISO15765 flow-control filter: " +
            "7E8 -> 7E0"
        );
    }

    private static void AssertEqual(
        string name,
        double expected,
        double actual)
    {
        const double tolerance = 0.01;

        if (Math.Abs(expected - actual) >
            tolerance)
        {
            Console.WriteLine(
                $"[FAIL] {name}: " +
                $"expected {expected:F2}, " +
                $"got {actual:F2}"
            );

            throw new Exception(
                $"{name} self-test failed."
            );
        }

        Console.WriteLine(
            $"[PASS] {name}: {actual:F2}"
        );
    }
}