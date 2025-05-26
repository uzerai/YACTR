using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YACTR.Model.Authentication;

namespace YACTR.Model.Organizations;

public class OrganizationTeam : BaseEntity
{
    [Required]
    public required string Name { get; set; }
    [Required]
    [ForeignKey("Organization")]
    public required Guid OrganizationId { get; set; }

    public virtual Organization Organization { get; set; } = null!;
    public virtual ICollection<OrganizationTeamUser> OrganizationTeamUsers { get; set; } = [];
    public virtual ICollection<User> Users { get; set; } = [];
}
