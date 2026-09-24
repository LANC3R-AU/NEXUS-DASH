using Nexus.J2534.Bridge.VehicleProfiles.Mitsubishi;

namespace Nexus.J2534.Bridge.VehicleProfiles;

public static class VehicleProfileRegistry
{
    private static readonly IReadOnlyList<IVehicleProfile> Profiles =
        new IVehicleProfile[]
        {
            new GenericObd2Profile(),
            new Lancer2009Profile()
        };

    public static IReadOnlyList<IVehicleProfile> GetAll()
    {
        return Profiles;
    }

    public static IVehicleProfile? GetById(string id)
    {
        return Profiles.FirstOrDefault(
            profile =>
                string.Equals(
                    profile.Id,
                    id,
                    StringComparison.OrdinalIgnoreCase
                )
        );
    }

    public static IVehicleProfile GetDefault()
    {
        return GetById("generic-obd2")
            ?? throw new InvalidOperationException(
                "Generic OBD-II profile is missing."
            );
    }
}