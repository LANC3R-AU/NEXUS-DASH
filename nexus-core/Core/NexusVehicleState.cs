namespace Nexus.J2534.Bridge.Core;

/// <summary>
/// Normalized real-time vehicle state used throughout
/// NEXUS CORE and NEXUS DASH.
///
/// Hardware adapters and vehicle profiles feed this model.
/// The dashboard should never need to understand raw CAN,
/// J2534, OBD-II or manufacturer-specific messages.
/// </summary>
public sealed record NexusVehicleState
{
    // --------------------------------------------------
    // POWERTRAIN
    // --------------------------------------------------

    public double? Rpm { get; init; }

    public double? SpeedKph { get; init; }

    public double? CoolantTempC { get; init; }

    public double? FuelPercent { get; init; }

    public string Gear { get; init; } = "-";


    // --------------------------------------------------
    // VEHICLE STATE
    // --------------------------------------------------

    public bool IgnitionOn { get; init; }

    public bool EngineRunning { get; init; }


    // --------------------------------------------------
    // DRIVER INDICATORS
    // --------------------------------------------------

    public bool LeftIndicator { get; init; }

    public bool RightIndicator { get; init; }

    public bool HighBeam { get; init; }

    public bool ParkingBrake { get; init; }


    // --------------------------------------------------
    // WARNING INDICATORS
    // --------------------------------------------------

    public bool CheckEngine { get; init; }

    public bool AbsWarning { get; init; }

    public bool TractionControlWarning { get; init; }

    public bool BatteryWarning { get; init; }

    public bool OilPressureWarning { get; init; }


    // --------------------------------------------------
    // VEHICLE INFORMATION
    // --------------------------------------------------

    public double? OdometerKm { get; init; }


    // --------------------------------------------------
    // CONNECTION / DATA HEALTH
    // --------------------------------------------------

    public bool Connected { get; init; }

    public string? Source { get; init; }

    public long? LastUpdate { get; init; }


    // --------------------------------------------------
    // FACTORY STATES
    // --------------------------------------------------

    /// <summary>
    /// Safe state used before vehicle communication
    /// has been established.
    ///
    /// Unknown measurements remain null instead of
    /// pretending the vehicle is reporting zero.
    /// </summary>
    public static NexusVehicleState Disconnected =>
        new()
        {
            Rpm = null,
            SpeedKph = null,
            CoolantTempC = null,
            FuelPercent = null,

            Gear = "-",

            IgnitionOn = false,
            EngineRunning = false,

            LeftIndicator = false,
            RightIndicator = false,
            HighBeam = false,
            ParkingBrake = false,

            CheckEngine = false,
            AbsWarning = false,
            TractionControlWarning = false,
            BatteryWarning = false,
            OilPressureWarning = false,

            OdometerKm = null,

            Connected = false,
            Source = null,
            LastUpdate = null
        };
}