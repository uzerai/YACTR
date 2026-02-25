using FastEndpoints;

namespace YACTR.Api.Endpoints.Images;

public class ImagesEndpointGroup : Group
{
    public ImagesEndpointGroup()
    {
        Configure("images", ep => { });
    }
}