using FastEndpoints;

namespace YACTR.Api.Endpoints.Sectors;

public class SectorsEndpointGroup : Group
{
    public SectorsEndpointGroup()
    {
        Configure("sectors", ep => { });
    }
}