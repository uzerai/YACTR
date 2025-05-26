using YACTR.Model.Authentication;

namespace YACTR.DI.Authorization.UserContext;

public interface IUserContext
{
    User? CurrentUser { get; }
    bool IsAuthenticated { get; }
} 