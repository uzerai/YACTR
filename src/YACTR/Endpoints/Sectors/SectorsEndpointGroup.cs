using FastEndpoints;

namespace YACTR.Endpoints.Sectors;

public class SectorsEndpointGroup : Group
{
    public SectorsEndpointGroup()
    {
        Configure("sectors", ep => { });
    }
}