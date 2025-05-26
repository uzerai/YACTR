using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using YACTR.Data.Model.Authentication;

namespace YACTR.Data.Model;

public class Image : BaseEntity
{
    public required string Key { get; set; }
    public required string Bucket { get; set; }
    public string? Description { get; set; }
    [JsonIgnore]
    [ForeignKey("Uploader")]
    public Guid? UploaderId { get; set; }
    [JsonIgnore]
    public virtual User? Uploader { get; set; }
    [JsonIgnore]
    public Guid? RelatedEntityId { get; set; }
    [JsonIgnore]
    public virtual BaseEntity? RelatedEntity { get; set; }
}
