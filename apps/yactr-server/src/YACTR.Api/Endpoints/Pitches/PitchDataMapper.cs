using FastEndpoints;
using YACTR.Domain.Model.Climbing;

namespace YACTR.Api.Endpoints.Pitches;

public class PitchDataMapper : Mapper<PitchRequestData, PitchResponse, Pitch>
{
    public override Pitch ToEntity(PitchRequestData r) => new()
    {
        Name = r.Name,
        Type = r.Type,
        Description = r.Description,
        SectorId = r.SectorId,
        RouteId = r.RouteId,
    };

    public override PitchResponse FromEntity(Pitch e) => new(e.Id, e.SectorId, e.RouteId, e.Name, e.Type, e.Description, e.Grade, e.PitchOrder);

    public override Pitch UpdateEntity(PitchRequestData r, Pitch e) => new()
    {
        Name = r.Name,
        Type = r.Type,
        Description = r.Description,
    };
}