namespace MyMVCProject.Core.Application.Interface.Repositories
{
    public interface IFileRepository
    {
        Task<string> UploadAsync(IFormFile? file);
    }
}
