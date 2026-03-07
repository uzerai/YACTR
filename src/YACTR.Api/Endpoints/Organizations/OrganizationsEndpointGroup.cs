using FastEndpoints;

namespace YACTR.Api.Endpoints.Organizations;

public record OrganizationResponse(
    Guid Id,
    string Name
);

public sealed class OrganizationsEndpointGroup : Group
{
    public OrganizationsEndpointGroup()
    {
        Configure("organizations", ep => { });
    }
}