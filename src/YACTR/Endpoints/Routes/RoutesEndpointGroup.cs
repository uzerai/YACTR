using FastEndpoints;

namespace YACTR.Endpoints;

public class RoutesEndpointGroup : Group
{
    public RoutesEndpointGroup()
    {
        Configure("routes", ep => {});
    }
} 