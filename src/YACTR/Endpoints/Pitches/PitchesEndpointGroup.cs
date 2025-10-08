using FastEndpoints;
using YACTR.Data.Model.Climbing;

namespace YACTR.Endpoints.Pitches;

/// <summary>
/// Request data for creating and updating the <see cref="Pitch"/> entity.
/// </summary>
/// <param name="SectorId">The ID of the sector.</param>
/// <param name="Name">The name of the pitch.</param>
/// <param name="Type">The type of the pitch.</param>
/// <param name="Description">The description of the pitch.</param>
/// <param name="Grade">The grade of the pitch.</param>
public record PitchRequestData(
    Guid SectorId,
    Guid RouteId,
    string Name,
    ClimbingType Type,
    string? Description,
    string? Grade,
    ushort? PitchOrder = null);

public class PitchesEndpointGroup : Group
{
    public PitchesEndpointGroup()
    {
        Configure("pitches", ep => { });
    }
}