using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using YACTR.Data.Model.Authentication;
using YACTR.Data.Model.Authorization.Permissions;

namespace YACTR.Data.Model.Organizations;

[PrimaryKey(nameof(OrganizationId), nameof(UserId))]
public class OrganizationUser
{
    [Required]
    [ForeignKey("Organization")]
    public required Guid OrganizationId { get; set; }
    [Required]
    [ForeignKey("User")]
    public required Guid UserId { get; set; }
    [JsonIgnore]
    public virtual Organization Organization { get; set; } = null!;
    [JsonIgnore]
    public virtual User User { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Permission> Permissions { get; set; } = [];
    public virtual ICollection<OrganizationTeamUser> OrganizationTeamUsers { get; set; } = [];
}