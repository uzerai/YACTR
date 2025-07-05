using FastEndpoints;

namespace YACTR.Endpoints;

public class SectorsEndpointGroup : Group
{
    public SectorsEndpointGroup()
    {
        Configure("sectors", ep => {});
    }
} 