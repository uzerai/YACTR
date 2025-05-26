using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YACTR.DI.Repository.Interface;
using YACTR.Model.Authentication;
using YACTR.DI.Authorization.UserContext;

namespace YACTR.Controllers;

[Authorize]
[Route("users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IEntityRepository<User> _userRepository;
    private readonly IUserContext _userContext;

    public UsersController(IEntityRepository<User> userRepository, IUserContext userContext)
    {
        _userRepository = userRepository;
        _userContext = userContext;
    }

    [HttpGet("me")]
    public IActionResult Get()
    {
        return Ok(_userContext.CurrentUser);
    }
}