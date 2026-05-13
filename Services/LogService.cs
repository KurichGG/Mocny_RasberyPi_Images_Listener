using Mocny_RasberyPi_Images_Listener.Data;
using Mocny_RasberyPi_Images_Listener.Models;
using Microsoft.EntityFrameworkCore;

namespace Mocny_RasberyPi_Images_Listener.Services
{
    public class LogService
    {
        private readonly AppDbContext _context;

        public LogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AuditLog>> GetAllLogs(int? limit = 100)
        {
            return await _context.AuditLogs
                .Include(al => al.User)
                .OrderByDescending(al => al.Timestamp)
                .Take(limit ?? 100)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetLogsByScreen(int screenId, int? limit = 50)
        {
            return await _context.AuditLogs
                .Where(al => al.EntityType == "Screen" && al.EntityId == screenId)
                .Include(al => al.User)
                .OrderByDescending(al => al.Timestamp)
                .Take(limit ?? 50)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetLogsByUser(int userId, int? limit = 50)
        {
            return await _context.AuditLogs
                .Where(al => al.UserId == userId)
                .Include(al => al.User)
                .OrderByDescending(al => al.Timestamp)
                .Take(limit ?? 50)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetLogsByAction(string action, int? limit = 50)
        {
            return await _context.AuditLogs
                .Where(al => al.Action == action)
                .Include(al => al.User)
                .OrderByDescending(al => al.Timestamp)
                .Take(limit ?? 50)
                .ToListAsync();
        }

        public async Task LogAction(int? userId, string action, string entityType, int? entityId, string description)
        {
            var log = new AuditLog
            {
                UserId = userId,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Timestamp = DateTime.UtcNow,
                Description = description
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}