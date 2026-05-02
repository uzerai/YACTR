using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using YACTR.Domain.Model.Authentication;
using YACTR.Domain.Model.Authorization.Permissions;

namespace YACTR.Domain.Model.Organizations;

public class OrganizationUser
{
    [Required]
    public required Guid OrganizationId { get; set; }
    [Required]
    public required Guid UserId { get; set; }
    [JsonIgnore]
    public virtual Organization Organization { get; set; } = null!;
    [JsonIgnore]
    public virtual User User { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Permission> Permissions { get; set; } = DefaultUserPermissions.OrganizationPermissions;
    public virtual ICollection<OrganizationTeamUser> OrganizationTeamUsers { get; set; } = [];
}