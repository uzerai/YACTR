using FastEndpoints;

namespace YACTR.Endpoints.Organizations;

public class OrganizationsEndpointGroup : Group
{
    public OrganizationsEndpointGroup()
    {
        Configure("organizations", ep => { });
    }
}