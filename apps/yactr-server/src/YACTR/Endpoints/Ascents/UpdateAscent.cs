using FastEndpoints;
using NodaTime;
using YACTR.Data.Model.Achievement;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.UserContext;
using Microsoft.EntityFrameworkCore;

namespace YACTR.Endpoints;

public record UpdateAscentRequest(
    Guid AscentId,
    AscentType Type,
    Instant CompletedAt
);

public class UpdateAscent : Endpoint<UpdateAscentRequest, EmptyResponse>
{
    private readonly IRepository<Ascent> _ascentRepository;
    private readonly IUserContext _userContext;

    public UpdateAscent(
        IRepository<Ascent> ascentRepository,
        IUserContext userContext)
    {
        _ascentRepository = ascentRepository;
        _userContext = userContext;
    }

    public override void Configure()
    {
        Put("/{AscentId}");
        Group<AscentsEndpointGroup>();
    }

    public override async Task HandleAsync(UpdateAscentRequest req, CancellationToken ct)
    {
        var currentUser = _userContext.CurrentUser!;

        var ascent = await _ascentRepository.BuildTrackedQuery()
            .FirstOrDefaultAsync(a => a.Id == req.AscentId, ct);
        
        if (ascent == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        // Ensure the user owns this ascent
        if (ascent.UserId != currentUser.Id)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        ascent.Type = req.Type;
        ascent.CompletedAt = req.CompletedAt;
        await _ascentRepository.UpdateAsync(ascent, ct);
        await SendNoContentAsync(ct);
    }
}
