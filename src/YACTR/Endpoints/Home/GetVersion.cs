using System.Net;
using FastEndpoints;

[HttpGet("/")]
public class GetVersion : Endpoint<EmptyRequest, EmptyResponse>
{
  public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
  {
    await SendAsync(new(), (int)HttpStatusCode.OK, ct);
  }
}