using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YACTR.Data.Model.Authentication;
using YACTR.DI.Authorization.UserContext;
using YACTR.Data.Repository.Interface;

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