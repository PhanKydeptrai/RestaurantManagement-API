using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;

namespace RestaurantManagement.Application.Services;

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;
    private readonly IConfiguration _configuration;
    public CloudinaryService(IConfiguration configuration)
    {
        _configuration = configuration;
        _cloudinary = new Cloudinary(_configuration["Cloudinary"]);
        _cloudinary.Api.Secure = true;
    }

    //Upload image to cloudinary
    public async Task<ImageUploadResult> UploadAsync(MemoryStream memoryStream, string fileName)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, memoryStream),
            UploadPreset = "iiwd8tcu",
            Transformation = new Transformation()
                .Width(300)
                .Height(300)
                .Crop("fill")
                .Chain()
                .Quality("auto")
                .Chain()
                .FetchFormat("auto")
        };
        return await _cloudinary.UploadAsync(uploadParams);
    }

    public async Task<ImageUploadResult> UploadAsyncCustomePreset(MemoryStream memoryStream, string fileName, string uploadPreset)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, memoryStream),
            UploadPreset = uploadPreset
            
        };
        return await _cloudinary.UploadAsync(uploadParams);
    }

    //Delete image from cloudinary
    public async Task<DeletionResult> DeleteAsync(string imageUrl)
    {

        var deleteParams = new DeletionParams(ExtractPartFromUrl(imageUrl));
        return await _cloudinary.DestroyAsync(deleteParams);
        //Xử lý xóa 
        //var deleteParams = new DelResParams()
        //{
        //    PublicIds = new List<string> { imageId },
        //    Type = "upload",
        //    ResourceType = ResourceType.Image
        //};

        //var result = _cloudinary.DeleteResources(deleteParams);
        //Console.WriteLine(result.JsonObj);
    }

    public string ExtractPartFromUrl(string imageUrl) //Lấy id ảnh từ url
    {
        Uri uri = new Uri(imageUrl);
        string path = uri.AbsolutePath;
        string[] segments = path.Split('/');
        string desiredPart = segments[segments.Length - 1].Split('.')[0];
        return desiredPart;
    }
}
