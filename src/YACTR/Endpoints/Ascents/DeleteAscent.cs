using FastEndpoints;
using YACTR.Data.Model.Achievement;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.UserContext;
using Microsoft.EntityFrameworkCore;

namespace YACTR.Endpoints;

public record DeleteAscentRequest(Guid AscentId);

public class DeleteAscent : Endpoint<DeleteAscentRequest, AscentResponse>
{
    private readonly IRepository<Ascent> _ascentRepository;
    private readonly IUserContext _userContext;

    public DeleteAscent(
        IRepository<Ascent> ascentRepository,
        IUserContext userContext)
    {
        _ascentRepository = ascentRepository;
        _userContext = userContext;
    }

    public override void Configure()
    {
        Delete("/{AscentId}");
        Group<AscentsEndpointGroup>();
    }

    public override async Task HandleAsync(DeleteAscentRequest req, CancellationToken ct)
    {
        var currentUser = _userContext.CurrentUser!;

        var ascent = await _ascentRepository.BuildTrackedQuery()
            .Include(a => a.Route)
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

        await _ascentRepository.DeleteAsync(ascent, ct);
        await SendAsync(new AscentResponse(
            Id: ascent.Id,
            UserId: ascent.UserId,
            Type: ascent.Type,
            CompletedAt: ascent.CompletedAt,
            Route: ascent.Route
        ), cancellation: ct);
    }
}
