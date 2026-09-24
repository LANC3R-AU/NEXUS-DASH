namespace Nexus.J2534.Bridge.Adapters;

/// <summary>
/// Creates vehicle-interface implementations for NEXUS CORE.
///
/// NEXUS CORE communicates through this factory so the
/// rest of the system does not need to know which physical
/// vehicle interface is being used.
/// </summary>
public static class VehicleInterfaceFactory
{
    public const string J2534 = "j2534";
    public const string NexusLink = "nexus-link";

    public static IVehicleInterface Create(
        string interfaceType,
        string? functionLibrary = null)
    {
        if (string.IsNullOrWhiteSpace(
                interfaceType))
        {
            throw new ArgumentException(
                "Vehicle interface type cannot be empty.",
                nameof(interfaceType)
            );
        }

        return interfaceType
            .Trim()
            .ToLowerInvariant() switch
        {
            J2534 =>
                CreateJ2534(
                    functionLibrary
                ),

            NexusLink =>
                CreateNexusLink(),

            _ =>
                throw new NotSupportedException(
                    $"Unknown NEXUS vehicle interface: " +
                    $"'{interfaceType}'."
                )
        };
    }

    private static IVehicleInterface CreateJ2534(
        string? functionLibrary)
    {
        if (string.IsNullOrWhiteSpace(
                functionLibrary))
        {
            throw new ArgumentException(
                "A J2534 FunctionLibrary path " +
                "is required."
            );
        }

        return new J2534VehicleInterface(
            functionLibrary
        );
    }

    private static IVehicleInterface CreateNexusLink()
    {
        return new NexusLinkVehicleInterface();
    }
}