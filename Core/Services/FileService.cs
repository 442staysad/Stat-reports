using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly string _rootPath;

        public FileService()
        {
            _rootPath = Path.Combine(Directory.GetCurrentDirectory(), "Reports");

            if (!Directory.Exists(_rootPath))
                Directory.CreateDirectory(_rootPath);
        }
        
        public async Task<string> SaveFileAsync(IFormFile file, string baseFolder, string branchName = null, int year = 0, string templateName = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Файл отсутствует или пуст");

            try
            {
                string folderPath;

                if (baseFolder == "Reports" && branchName != null && year > 0 && templateName != null)
                {
                    folderPath = Path.Combine(_rootPath, branchName, year.ToString(), templateName);
                }
                else if (baseFolder == "Templates")
                {
                    folderPath = Path.Combine(_rootPath, "Templates");
                }
                else
                {
                    throw new ArgumentException("Некорректный путь сохранения файла");
                }

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // Получаем расширение файла и имя без расширения
                string originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                string fileExtension = Path.GetExtension(file.FileName);

                // Если это "Reports", добавляем branchName_ перед именем файла
                string fileName = baseFolder == "Reports" && !string.IsNullOrEmpty(branchName)
                    ? $"{branchName}_{originalFileName}{fileExtension}"
                    : file.FileName;

                string fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return fullPath;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка при сохранении файла: {ex.Message}", ex);
            }
        }

        public async Task<byte[]> GetFileAsync(string filePath)
        {
            var fullPath = Path.Combine(_rootPath, filePath);
            if (!File.Exists(fullPath))
                return null;

            return await File.ReadAllBytesAsync(fullPath); 
        }

        public Task<bool> DeleteFileAsync(string filePath)
        {
            var fullPath = Path.Combine(_rootPath, filePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}