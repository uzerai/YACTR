using FastEndpoints;
using YACTR.Data.Model.Location;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints;

public class CreatePitch : Endpoint<PitchRequestData, Pitch>
{
    private readonly IEntityRepository<Pitch> _pitchRepository;

    public CreatePitch(IEntityRepository<Pitch> pitchRepository)
    {
        _pitchRepository = pitchRepository;
    }

    public override void Configure()
    {
        Post("/");
        Group<PitchesEndpointGroup>();
    }

    public override async Task HandleAsync(PitchRequestData req, CancellationToken ct)
    {
        var createdPitch = await _pitchRepository.CreateAsync(new()
        {
            Name = req.Name,
            Description = req.Description,
            Type = req.Type,
            SectorId = req.SectorId
        }, ct);

        await SendCreatedAtAsync<GetPitchById>(createdPitch.Id, createdPitch, cancellation: ct);
    }
} 