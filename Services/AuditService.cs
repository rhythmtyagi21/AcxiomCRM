using AcxiomCRM.Backend.Data;
using AcxiomCRM.Backend.Models;
using System.Security.Claims;
namespace AcxiomCRM.Backend.Services;
public static class AuditService
{
    public static async Task WriteAsync(CrmDbContext db, ClaimsPrincipal user, HttpContext context, string action, string entity, string record)
    {
        var name = user.Identity?.Name ?? "Anonymous";
        db.AuditLogs.Add(new AuditLog { User = name, Action = action, Entity = entity, Record = record, Date = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() });
        await db.SaveChangesAsync();
    }
}
