using FastEndpoints;

namespace YACTR.Endpoints.Users;

public class UsersEndpointGroup : Group
{
    public UsersEndpointGroup()
    {
        Configure("users", ep => { });
    }
}