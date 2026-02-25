using FastEndpoints;

namespace YACTR.Api.Endpoints.Routes;

public class RoutesEndpointGroup : Group
{
    public RoutesEndpointGroup()
    {
        Configure("routes", ep => { });
    }
}