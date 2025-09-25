using System.Net;
using FastEndpoints;

namespace YACTR.Endpoints.Root;

public class GetIndex : Endpoint<EmptyRequest, EmptyResponse>
{
    public override void Configure()
    {
        Get("/");
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        await SendAsync(new(), (int)HttpStatusCode.NoContent, ct);
    }
}