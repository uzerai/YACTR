using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using YACTR.Data.Model.Authentication;
using YACTR.Data.Model.Authorization.Permissions;

namespace YACTR.Data.Model.Organizations;

[PrimaryKey(nameof(OrganizationId), nameof(OrganizationTeamId), nameof(UserId))]
public class OrganizationTeamUser
{
    [Required]
    [ForeignKey("Organization OrganizationUser")]
    public Guid OrganizationId { get; set; }
    [Required]
    [ForeignKey("User OrganizationUser")]
    public Guid UserId { get; set; }
    [Required]
    [ForeignKey("OrganizationTeam")]
    public Guid OrganizationTeamId { get; set; }
    [Column("permissions", TypeName = "jsonb")]
    public virtual ICollection<Permission> Permissions { get; set; } = DefaultUserPermissions.TeamPermissions;

    public virtual User User { get; set; } = null!;
    public virtual Organization Organization { get; set; } = null!;
    public virtual OrganizationUser OrganizationUser { get; set; } = null!;
    public virtual OrganizationTeam OrganizationTeam { get; set; } = null!;
}