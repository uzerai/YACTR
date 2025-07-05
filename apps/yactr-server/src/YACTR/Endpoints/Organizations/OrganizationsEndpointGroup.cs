using FastEndpoints;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace YACTR.Endpoints;

public class OrganizationsEndpointGroup : Group
{
  public OrganizationsEndpointGroup()
  {
    Configure("organizations", ep => {});
  }
}