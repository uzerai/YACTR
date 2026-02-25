using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YACTR.Domain.Model.Authentication;
using YACTR.Domain.Model.Authorization.Permissions;

namespace YACTR.Domain.Model.Organizations;

public class OrganizationTeamUser
{
    [Required]
    public Guid OrganizationId { get; set; }
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public Guid OrganizationTeamId { get; set; }
    [Column("permissions", TypeName = "jsonb")]
    public virtual ICollection<Permission> Permissions { get; set; } = DefaultUserPermissions.TeamPermissions;

    [Required]
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
    [Required]
    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; } = null!;
    [Required]
    [ForeignKey("UserId OrganizationId")]
    public virtual OrganizationUser OrganizationUser { get; set; } = null!;
    [Required]
    [ForeignKey("OrganizationTeamId")]
    public virtual OrganizationTeam OrganizationTeam { get; set; } = null!;
}