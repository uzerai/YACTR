using FastEndpoints;

namespace YACTR.Endpoints.Images;

public class ImagesEndpointGroup : Group
{
    public ImagesEndpointGroup()
    {
        Configure("images", ep => { });
    }
}