using FastEndpoints;

namespace YACTR.Endpoints.Areas;

public class AreasEndpointGroup : Group
{
    public AreasEndpointGroup()
    {
        Configure("areas", ep => { });
    }
}