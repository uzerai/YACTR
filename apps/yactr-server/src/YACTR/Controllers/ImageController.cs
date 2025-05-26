using YACTR.DI;
using YACTR.DI.Service;
using YACTR.Data.Model;
using Microsoft.AspNetCore.Mvc;
using YACTR.DI.Authorization.UserContext;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.DI.Authorization.Permissions;

namespace YACTR.Controllers;

[ApiController]
[Route("images")]
public class ImageController : ControllerBase
{ 
    private readonly IImageStorageService _imageStorageService;
    private readonly IUserContext _userContext;

    public ImageController(
        IImageStorageService imageStorageService,
        IUserContext userContext)
    {
        _imageStorageService = imageStorageService;
        _userContext = userContext;
    }
    
    [PlatformPermissionRequired(Permission.ImagesWrite)]
    [HttpPost]
    public async Task<ActionResult<Image>> UploadImage()
    {
        using var memoryStream = new MemoryStream();
        await Request.Body.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var uploadedImage = await _imageStorageService.UploadImage(
            memoryStream, 
            _userContext.CurrentUser!, 
            Guid.Empty);

        return Created(uploadedImage.Id.ToString(), uploadedImage);
    }
}
