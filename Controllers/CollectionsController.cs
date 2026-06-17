using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mocny_RasberyPi_Images_Listener.DTOs;
using Mocny_RasberyPi_Images_Listener.Services;

namespace Mocny_RasberyPi_Images_Listener.Controllers
{
    [ApiController]
    [Route("api/collections")]
    [Authorize]
    public class CollectionsController : ControllerBase
    {
        private readonly CollectionService _collectionService;

        public CollectionsController(CollectionService collectionService)
        {
            _collectionService = collectionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCollections()
        {
            var collections = await _collectionService.GetAllCollections();
            return Ok(collections);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCollectionById(int id)
        {
            var collection = await _collectionService.GetCollectionById(id);
            if (collection == null)
                return NotFound(new { message = "Collection not found" });

            return Ok(collection);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCollection([FromBody] CreateCollectionRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
                return BadRequest(new { message = "Name required" });

            var userId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : (int?)null;
            var collection = await _collectionService.CreateCollection(request.Name, userId);
            return CreatedAtAction(nameof(GetCollectionById), new { id = collection.Id }, collection);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCollection(int id, [FromBody] CreateCollectionRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
                return BadRequest(new { message = "Name required" });

            var result = await _collectionService.UpdateCollection(id, request.Name);
            if (!result)
                return NotFound(new { message = "Collection not found" });

            var updated = await _collectionService.GetCollectionById(id);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCollection(int id)
        {
            var result = await _collectionService.DeleteCollection(id);
            if (!result)
                return NotFound(new { message = "Collection not found" });

            return NoContent();
        }

        [HttpPost("{collectionId}/items")]
        public async Task<IActionResult> AddItemToCollection(int collectionId, [FromBody] AddCollectionItemRequest request)
        {
            var item = await _collectionService.AddItemToCollection(collectionId, request.ImageId, request.Order, request.DisplayDurationSeconds);
            if (item == null)
                return BadRequest(new { message = "Failed to add item to collection" });

            return Ok(item);
        }

        [HttpPut("{collectionId}/items/{itemId}")]
        public async Task<IActionResult> UpdateItemInCollection(int collectionId, int itemId, [FromBody] UpdateCollectionItemRequest request)
        {
            var result = await _collectionService.UpdateCollectionItem(collectionId, itemId, request.Order, request.DisplayDurationSeconds);
            if (!result)
                return NotFound(new { message = "Item not found in collection" });

            return NoContent();
        }

        [HttpDelete("{collectionId}/items/{itemId}")]
        public async Task<IActionResult> RemoveItemFromCollection(int collectionId, int itemId)
        {
            var result = await _collectionService.RemoveItemFromCollection(collectionId, itemId);
            if (!result)
                return NotFound(new { message = "Item not found in collection" });

            return NoContent();
        }
    }

    public class CreateCollectionRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    public class AddCollectionItemRequest
    {
        public int ImageId { get; set; }
        public int Order { get; set; }
        public int DisplayDurationSeconds { get; set; } = 5;
    }
}