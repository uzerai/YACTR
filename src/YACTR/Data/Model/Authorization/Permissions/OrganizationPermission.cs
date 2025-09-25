using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using YACTR.Data.Model.Authentication;
using YACTR.Data.Model.Organizations;

namespace YACTR.Data.Model.Authorization.Permissions;

[PrimaryKey(nameof(OrganizationId), nameof(UserId), nameof(Permission))]
public class OrganizationPermission
{
    public Permission Permission { get; set; }
    [JsonIgnore]
    [ForeignKey("Organization")]
    public Guid OrganizationId { get; set; }
    [JsonIgnore]
    [ForeignKey("User")]
    public Guid UserId { get; set; }

    [JsonIgnore]
    public virtual OrganizationUser OrganizationUser { get; set; } = null!;
    [JsonIgnore]
    public virtual Organization Organization { get; set; } = null!;
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}