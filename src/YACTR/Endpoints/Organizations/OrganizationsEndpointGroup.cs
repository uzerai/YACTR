using FastEndpoints;

namespace YACTR.Endpoints;

public class OrganizationsEndpointGroup : Group
{
  public OrganizationsEndpointGroup()
  {
    Configure("organizations", ep => {});
  }
}