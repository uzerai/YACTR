using Microsoft.EntityFrameworkCore;
using YACTR.Domain.Model;

namespace YACTR.Infrastructure.Database.Table;

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