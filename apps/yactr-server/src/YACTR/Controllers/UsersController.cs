using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YACTR.DI.Authorization.UserContext;

namespace YACTR.Controllers;

[Authorize]
[Route("users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserContext _userContext;

    public UsersController(IUserContext userContext)
    {
        _userContext = userContext;
    }

    [HttpGet("me")]
    public IActionResult Get()
    {
        return Ok(_userContext.CurrentUser);
    }
}