using FastEndpoints;

namespace YACTR.Endpoints;

public class UsersEndpointGroup : Group
{
    public UsersEndpointGroup()
    {
        Configure("users", ep => {});
    }
} 