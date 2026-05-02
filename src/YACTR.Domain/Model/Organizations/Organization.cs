using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using YACTR.Domain.Model.Authentication;

namespace YACTR.Domain.Model.Organizations;

public class Organization : BaseEntity
{
    [Required]
    public required string Name { get; set; }
    [JsonIgnore]
    public virtual ICollection<User> Users { get; set; } = [];
    [JsonIgnore]
    public virtual ICollection<OrganizationUser> OrganizationUsers { get; set; } = [];
    [JsonIgnore]
    public virtual ICollection<OrganizationTeam> Teams { get; set; } = [];
}