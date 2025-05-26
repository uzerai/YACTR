using YACTR.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace YACTR.Data.ConfigurationExtension;

public static class ImageConfigurationExtension
{
    public static ModelBuilder ConfigureImageModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Image>()
            .HasOne(e => e.Uploader)
            .WithMany()
            .HasForeignKey(e => e.UploaderId);

        return modelBuilder;
    }
}