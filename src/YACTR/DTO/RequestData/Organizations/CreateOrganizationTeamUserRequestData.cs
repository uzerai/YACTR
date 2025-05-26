using YACTR.Model.Authorization.Permissions;

namespace YACTR.DTO.RequestData.Organizations;

public class CreateOrganizationTeamUserRequestData
{
    public required Guid UserId { get; set; }
    public required Guid OrganizationTeamId { get; set; }
    public required List<Permission> Permissions { get; set; }
}
