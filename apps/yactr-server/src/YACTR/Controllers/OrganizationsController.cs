using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YACTR.DI.Authorization.Permissions;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Model.Organizations;
using YACTR.DTO.RequestData.Organizations;
using YACTR.DI.Authorization.UserContext;
using YACTR.Data.Repository.Interface;
namespace YACTR.Controllers;

[Authorize]
[Route("organizations")]
[ApiController]
public class OrganizationsController : ControllerBase
{
    private readonly IEntityRepository<Organization> _organizationRepository;
    private readonly IRepository<OrganizationUser> _organizationUserRepository;
    private readonly IUserContext _userContext;

    public OrganizationsController(
        IEntityRepository<Organization> organizationRepository,
        IRepository<OrganizationUser> organizationUserRepository,
        IUserContext userContext)
    {
        _organizationRepository = organizationRepository;
        _organizationUserRepository = organizationUserRepository;
        _userContext = userContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {   
        return Ok(await _organizationRepository.GetAllAsync());
    }

    [HttpGet("{organizationId}")]
    [OrganizationPermissionRequired(Permission.OrganizationsRead)]
    public async Task<IActionResult> Get([FromRoute][Required] Guid organizationId)
    {
        return Ok(await _organizationRepository.GetByIdAsync(organizationId));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationRequestData createOrganizationRequest)
    {
        var newOrganization = new Organization
        {
            Name = createOrganizationRequest.Name,
        };

        var createdOrganization = await _organizationRepository.CreateAsync(newOrganization);
        var organizationUser = new OrganizationUser
        {
            OrganizationId = createdOrganization.Id,
            UserId = _userContext.CurrentUser!.Id,
            Permissions = Enum.GetValues<Permission>()
                .Select(permission => new OrganizationPermission
                {
                    Permission = permission
                }).ToList()
        };

        await _organizationUserRepository.CreateAsync(organizationUser);

        return Ok(createdOrganization);
    }
}
