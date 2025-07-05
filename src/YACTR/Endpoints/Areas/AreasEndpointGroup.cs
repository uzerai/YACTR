using FastEndpoints;

namespace YACTR.Endpoints;

public class AreasEndpointGroup : Group
{
    public AreasEndpointGroup()
    {
        Configure("areas", ep => {});
    }
} 