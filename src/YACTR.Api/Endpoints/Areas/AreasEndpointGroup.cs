using FastEndpoints;

namespace YACTR.Api.Endpoints.Areas;

public class AreasEndpointGroup : Group
{
    public AreasEndpointGroup()
    {
        Configure("areas", ep => { });
    }
}