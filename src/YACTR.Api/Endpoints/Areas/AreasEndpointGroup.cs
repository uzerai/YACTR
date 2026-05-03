using FastEndpoints;

namespace YACTR.Api.Endpoints.Areas;

public sealed class AreasEndpointGroup : Group
{
    public AreasEndpointGroup()
    {
        Configure("areas", ep => { });
    }
}