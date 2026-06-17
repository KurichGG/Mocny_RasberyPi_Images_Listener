using Mocny_RasberyPi_Images_Listener.Data;
using Mocny_RasberyPi_Images_Listener.DTOs;
using Mocny_RasberyPi_Images_Listener.Models;
using Microsoft.EntityFrameworkCore;

namespace Mocny_RasberyPi_Images_Listener.Services
{
    public class CollectionService
    {
        private readonly AppDbContext _context;

        public CollectionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CollectionDto>> GetAllCollections()
        {
            return await _context.Collections
                .Include(c => c.Items)
                .Select(c => new CollectionDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ItemCount = c.Items.Count,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<CollectionDto?> GetCollectionById(int id)
        {
            var collection = await _context.Collections
                .Include(c => c.Items)
                    .ThenInclude(i => i.Image)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (collection == null) return null;

            return new CollectionDto
            {
                Id = collection.Id,
                Name = collection.Name,
                ItemCount = collection.Items.Count,
                CreatedAt = collection.CreatedAt,
                Items = collection.Items
                    .OrderBy(i => i.Order)
                    .Select(i => new CollectionItemDto
                    {
                        Id = i.Id,
                        ImageId = i.ImageId,
                        ImageName = i.Image.Name,
                        ThumbnailPath = i.Image.ThumbnailPath,
                        Order = i.Order,
                        DisplayDurationSeconds = i.DisplayDurationSeconds
                    })
                    .ToList()
            };
        }

        public async Task<CollectionDto> CreateCollection(string name, int? createdBy)
        {
            var collection = new Collection
            {
                Name = name,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            _context.Collections.Add(collection);
            await _context.SaveChangesAsync();

            return new CollectionDto
            {
                Id = collection.Id,
                Name = collection.Name,
                ItemCount = 0,
                CreatedAt = collection.CreatedAt
            };
        }

        public async Task<bool> UpdateCollection(int id, string name)
        {
            var collection = await _context.Collections.FindAsync(id);
            if (collection == null) return false;

            collection.Name = name;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCollection(int id)
        {
            var collection = await _context.Collections.FindAsync(id);
            if (collection == null) return false;

            _context.Collections.Remove(collection);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CollectionItemDto?> AddItemToCollection(int collectionId, int imageId, int order, int displayDurationSeconds)
        {
            var collection = await _context.Collections.FindAsync(collectionId);
            if (collection == null) return null;

            var image = await _context.Images.FindAsync(imageId);
            if (image == null) return null;

            var item = new CollectionItem
            {
                CollectionId = collectionId,
                ImageId = imageId,
                Order = order,
                DisplayDurationSeconds = displayDurationSeconds
            };

            _context.CollectionItems.Add(item);
            await _context.SaveChangesAsync();

            return new CollectionItemDto
            {
                Id = item.Id,
                ImageId = image.Id,
                ImageName = image.Name,
                ThumbnailPath = image.ThumbnailPath,
                Order = item.Order,
                DisplayDurationSeconds = item.DisplayDurationSeconds
            };
        }

        public async Task<bool> UpdateCollectionItem(int collectionId, int itemId, int order, int displayDurationSeconds)
        {
            var item = await _context.CollectionItems.FindAsync(itemId);
            if (item == null || item.CollectionId != collectionId) return false;

            item.Order = order;
            item.DisplayDurationSeconds = displayDurationSeconds;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveItemFromCollection(int collectionId, int itemId)
        {
            var item = await _context.CollectionItems.FindAsync(itemId);
            if (item == null || item.CollectionId != collectionId) return false;

            _context.CollectionItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}