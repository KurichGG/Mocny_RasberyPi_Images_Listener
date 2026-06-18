using Mocny_RasberyPi_Images_Listener.Data;
using Mocny_RasberyPi_Images_Listener.DTOs;
using Mocny_RasberyPi_Images_Listener.Models;
using Microsoft.EntityFrameworkCore;

namespace Mocny_RasberyPi_Images_Listener.Services
{
    public class ImageService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly string _uploadsFolder;

        public ImageService(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;

            var webRoot = !string.IsNullOrEmpty(_environment.WebRootPath)
                ? _environment.WebRootPath
                : Path.Combine(_environment.ContentRootPath, "wwwroot");

            _uploadsFolder = Path.Combine(webRoot, "uploads", "images");

            if (!Directory.Exists(_uploadsFolder))
                Directory.CreateDirectory(_uploadsFolder);
        }

        public async Task<List<ImageDto>> GetAllImages()
        {
            return await _context.Images
                .Select(i => new ImageDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Format = i.Format,
                    Width = i.Width,
                    Height = i.Height,
                    FileSize = i.FileSize,
                    ThumbnailPath = i.ThumbnailPath,
                    CreatedAt = i.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<ImageDto> UploadImage(IFormFile file, int? uploadedBy)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException("Invalid file format");

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var image = new Image
            {
                Name = file.FileName,
                FilePath = $"/uploads/images/{fileName}",
                ThumbnailPath = $"/uploads/images/{fileName}",
                Format = fileExtension.TrimStart('.'),
                Width = 0,
                Height = 0,
                FileSize = file.Length,
                UploadedBy = uploadedBy,
                CreatedAt = DateTime.UtcNow
            };

            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            return new ImageDto
            {
                Id = image.Id,
                Name = image.Name,
                Format = image.Format,
                Width = image.Width,
                Height = image.Height,
                FileSize = image.FileSize,
                ThumbnailPath = image.ThumbnailPath,
                CreatedAt = image.CreatedAt
            };
        }

        public async Task<bool> DeleteImage(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null) return false;

            try
            {
                var fileName = Path.GetFileName(image.FilePath);
                var filePath = Path.Combine(_uploadsFolder, fileName);
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch { }

            _context.Images.Remove(image);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<FileStream?> GetImageFile(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null) return null;

            var fileName = Path.GetFileName(image.FilePath);
            var filePath = Path.Combine(_uploadsFolder, fileName);

            if (!File.Exists(filePath))
                return null;

            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }
    }
}