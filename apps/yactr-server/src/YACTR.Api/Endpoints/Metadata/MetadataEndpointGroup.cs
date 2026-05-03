using FastEndpoints;

namespace YACTR.Api.Endpoints.Metadata;

public sealed class MetadataEndpointGroup : Group
{
    public MetadataEndpointGroup()
    {
        Configure("metadata", ep => { });
    }
}