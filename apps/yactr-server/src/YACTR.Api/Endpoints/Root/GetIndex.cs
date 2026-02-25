using FastEndpoints;

namespace YACTR.Api.Endpoints.Root;

public class GetIndex : Endpoint<EmptyRequest, EmptyResponse>
{
    public override void Configure()
    {
        Get("/");
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        await Send.NoContentAsync(ct);
    }
}