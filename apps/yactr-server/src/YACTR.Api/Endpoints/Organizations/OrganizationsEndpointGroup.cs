using FastEndpoints;

namespace YACTR.Api.Endpoints.Organizations;

public sealed class OrganizationsEndpointGroup : Group
{
    public OrganizationsEndpointGroup()
    {
        Configure("organizations", ep => { });
    }
}