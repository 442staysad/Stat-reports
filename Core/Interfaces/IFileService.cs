using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folder);
        Task<byte[]> GetFileAsync(string filePath);
        Task<bool> DeleteFileAsync(string filePath);
    }
}