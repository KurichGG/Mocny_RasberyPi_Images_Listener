using Mocny_RasberyPi_Images_Listener.Data;
using Mocny_RasberyPi_Images_Listener.DTOs;
using Mocny_RasberyPi_Images_Listener.Models;
using Microsoft.EntityFrameworkCore;

namespace Mocny_RasberyPi_Images_Listener.Services
{
    public class ScreenService
    {
        private readonly AppDbContext _context;

        public ScreenService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ScreenDto>> GetAllScreens()
        {
            return await _context.Screens
                .Include(s => s.Group)
                .Select(s => new ScreenDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    UniqueIdentifier = s.UniqueIdentifier,
                    GroupId = s.GroupId,
                    Location = s.Location,
                    Status = s.Status,
                    LastSeen = s.LastSeen,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<ScreenDto?> GetScreenById(int id)
        {
            var screen = await _context.Screens.FindAsync(id);
            if (screen == null) return null;

            return new ScreenDto
            {
                Id = screen.Id,
                Name = screen.Name,
                UniqueIdentifier = screen.UniqueIdentifier,
                GroupId = screen.GroupId,
                Location = screen.Location,
                Status = screen.Status,
                LastSeen = screen.LastSeen,
                CreatedAt = screen.CreatedAt
            };
        }

        public async Task<ScreenDto> CreateScreen(CreateScreenRequest request)
        {
            var screen = new Screen
            {
                Name = request.Name,
                UniqueIdentifier = request.UniqueIdentifier,
                GroupId = request.GroupId,
                Location = request.Location,
                Status = "Offline",
                CreatedAt = DateTime.UtcNow
            };

            _context.Screens.Add(screen);
            await _context.SaveChangesAsync();

            return new ScreenDto
            {
                Id = screen.Id,
                Name = screen.Name,
                UniqueIdentifier = screen.UniqueIdentifier,
                GroupId = screen.GroupId,
                Location = screen.Location,
                Status = screen.Status,
                LastSeen = screen.LastSeen,
                CreatedAt = screen.CreatedAt
            };
        }

        public async Task<bool> UpdateScreen(int id, CreateScreenRequest request)
        {
            var screen = await _context.Screens.FindAsync(id);
            if (screen == null) return false;

            screen.Name = request.Name;
            screen.UniqueIdentifier = request.UniqueIdentifier;
            screen.GroupId = request.GroupId;
            screen.Location = request.Location;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteScreen(int id)
        {
            var screen = await _context.Screens.FindAsync(id);
            if (screen == null) return false;

            _context.Screens.Remove(screen);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetScreenPower(int id, bool isOnline)
        {
            var screen = await _context.Screens.FindAsync(id);
            if (screen == null) return false;

            screen.Status = isOnline ? "Online" : "Offline";
            screen.LastSeen = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}