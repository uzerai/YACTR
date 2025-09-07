using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Model.Climbing.Rating;
using YACTR.Data.Model.Organizations;

namespace YACTR.Data.Model.Authentication;

[Index(nameof(Auth0UserId), IsUnique = true)]
[Index(nameof(Email))]
public class User : BaseEntity
{
    [Required]
    [Column("auth0_user_id")]
    public required string Auth0UserId { get; init; }

    [Required]
    public required string Email { get; init; }

    [Required]
    public required string Username { get; init; }

    [Required]
    public Instant LastLogin { get; set; }

    // OrganizationUser relationships
    [Column("platform_permissions", TypeName = "jsonb")]
    public virtual ICollection<Permission> PlatformPermissions { get; set; } = DefaultUserPermissions.PlatformPermissions.ToList();
    [Column("admin_permissions", TypeName = "jsonb")]
    public virtual ICollection<Permission> AdminPermissions { get; set; } = [];
    public virtual ICollection<OrganizationUser> OrganizationUsers { get; set; } = [];
    public virtual ICollection<Organization> Organizations { get; set; } = [];

    [JsonIgnore]
    public virtual ICollection<RouteRating> RouteRatings { get; set; } = [];
    [JsonIgnore]
    public virtual ICollection<RouteLike> RouteLikes { get; set; } = [];
}