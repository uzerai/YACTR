using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using YACTR.Domain.Model.Authentication;

namespace YACTR.Domain.Model;

public class Image : BaseEntity
{
    public string? Description { get; set; }
    [JsonIgnore]
    public Guid? UploaderId { get; set; }
    [JsonIgnore]
    [ForeignKey("UploaderId")]
    public virtual User? Uploader { get; set; }
}
