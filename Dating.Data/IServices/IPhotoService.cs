using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Dating.Data.IServices;

public interface IPhotoService
{
    Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
    Task<DeletionResult> DeletePhotoAsync(string publicId);
}