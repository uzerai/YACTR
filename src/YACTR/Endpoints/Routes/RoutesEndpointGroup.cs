using FastEndpoints;

namespace YACTR.Endpoints.Routes;

public class RoutesEndpointGroup : Group
{
    public RoutesEndpointGroup()
    {
        Configure("routes", ep => { });
    }
}