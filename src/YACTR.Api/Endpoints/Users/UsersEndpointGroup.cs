using FastEndpoints;

namespace YACTR.Api.Endpoints.Users;

public class UsersEndpointGroup : Group
{
    public UsersEndpointGroup()
    {
        Configure("users", ep => { });
    }
}