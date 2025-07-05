using FastEndpoints;

namespace YACTR.Endpoints;

public class ImagesEndpointGroup : Group
{
    public ImagesEndpointGroup()
    {
        Configure("images", ep => {});
    }
} 