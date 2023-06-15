using Microsoft.AspNetCore.Http;

namespace SmallClientBusiness.Common.Dto;

public class AvatarUpload
{
    public IFormFile avatar { get; set; }
}