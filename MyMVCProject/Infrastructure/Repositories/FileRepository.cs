using Microsoft.Extensions.Options;
using MyMVCProject.Core.Application.Config;
using MyMVCProject.Core.Application.Interface.Repositories;

namespace MyMVCProject.Infrastructure.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly StorageConfig _config;
        public FileRepository(IOptions<StorageConfig> config)
        {
            _config = config.Value;
        }

        public async Task<string?> UploadAsync(IFormFile? file)
        {
            if(file == null)
            {
                return null;
            }

            var a = file.ContentType.Split('/');
            var newName = $"IMG{a[0]}{Guid.NewGuid().ToString().Substring(6, 5)}.{a[1]}";

            var b = _config.Path;
            if (!Directory.Exists(b))
            {
                Directory.CreateDirectory(b);
            }

            var c = Path.Combine(b, newName);

            using (var d = new FileStream(c, FileMode.Create))
            {
                await file.CopyToAsync(d);
            }

            return newName;
        }
    }
}
