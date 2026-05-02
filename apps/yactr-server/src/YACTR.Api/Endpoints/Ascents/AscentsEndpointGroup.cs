using FastEndpoints;

namespace YACTR.Api.Endpoints.Ascents;

public class AscentsEndpointGroup : Group
{
    public AscentsEndpointGroup()
    {
        Configure("ascents", ep => { });
    }
}