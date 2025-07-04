using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.Permissions;
using YACTR.DTO.RequestData.Organizations;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Model.Organizations;
using System.Net;
using NetTopologySuite.Operation.Buffer;

namespace YACTR.Controllers;

[Authorize]
[Route("organizations/{organizationId}/teams/{teamId}/users")]
[ApiController]
public class OrganizationTeamUsersController : ControllerBase
{
    private readonly IRepository<OrganizationTeamUser> _organizationTeamUserRepository;
    private readonly IEntityRepository<OrganizationTeam> _organizationTeamRepository;

    public OrganizationTeamUsersController(IRepository<OrganizationTeamUser> organizationTeamUserRepository,
        IEntityRepository<OrganizationTeam> organizationTeamRepository)
    {
        _organizationTeamUserRepository = organizationTeamUserRepository;
        _organizationTeamRepository = organizationTeamRepository;
    }

    [HttpPost]
    [OrganizationPermissionRequired(Permission.TeamsWrite)]
    public async Task<IActionResult> Create(
        [FromRoute][Required] Guid organizationId,
        [FromRoute][Required] Guid teamId,
        [FromBody][Required] CreateOrganizationTeamUserRequestData requestData)
    {
        var organizationTeamUser = new OrganizationTeamUser
        {
            OrganizationId = organizationId,
            OrganizationTeamId = teamId,
            UserId = requestData.UserId,
            Permissions = requestData.Permissions,
        };

        if (await _organizationTeamRepository.GetByIdAsync(teamId) is null)
        {
            return ValidationProblem("Team does not exist", teamId.ToString(), (int)HttpStatusCode.FailedDependency, "error");
        }

        await _organizationTeamUserRepository.CreateAsync(organizationTeamUser);

        return Ok(organizationTeamUser);
    }
}
