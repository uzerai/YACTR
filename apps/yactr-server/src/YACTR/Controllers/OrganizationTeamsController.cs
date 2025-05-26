using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YACTR.DI.Authorization.Permissions;
using YACTR.DI.Repository.Interface;
using YACTR.DTO.RequestData.Organizations;
using YACTR.Model.Authorization.Permissions;
using YACTR.Model.Organizations;

namespace YACTR.Controllers;

[Authorize]
[Route("organizations/{organizationId}/teams")]
[ApiController]
public class OrganizationTeamsController : ControllerBase
{
    private readonly IEntityRepository<OrganizationTeam> _organizationTeamRepository;

    public OrganizationTeamsController(IEntityRepository<OrganizationTeam> organizationTeamRepository)
    {
        _organizationTeamRepository = organizationTeamRepository;
    }

    [HttpGet]
    [OrganizationPermissionRequired(Permission.TeamsRead)]
    public async Task<IActionResult> GetAll([FromRoute][Required] Guid organizationId)
    {
        var teams = await _organizationTeamRepository.BuildReadonlyQuery()
            .Where(e => e.OrganizationId == organizationId)
            .ToListAsync();
        return Ok(teams);
    }

    [HttpPost]
    [OrganizationPermissionRequired(Permission.TeamsWrite)]
    public async Task<IActionResult> Create(
      [FromRoute][Required] Guid organizationId,
      [FromBody][Required] CreateOrganizationTeamRequestData requestData)
    {
        var team = new OrganizationTeam
        {
            Name = requestData.Name,
            OrganizationId = organizationId,
        };

        await _organizationTeamRepository.CreateAsync(team);

        return Ok(team);
    }
}