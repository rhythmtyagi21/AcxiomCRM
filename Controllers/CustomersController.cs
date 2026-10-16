using AcxiomCRM.Backend.Data;
using AcxiomCRM.Backend.Models;
using AcxiomCRM.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace AcxiomCRM.Backend.Controllers;
[ApiController, Route("api/customers"), Authorize]
public class CustomersController(CrmDbContext db) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get([FromQuery] string? search) { var q = db.Customers.AsQueryable(); if (!string.IsNullOrWhiteSpace(search)) q = q.Where(x => x.Name.Contains(search) || x.Email.Contains(search) || x.Phone.Contains(search) || x.Company.Contains(search)); if (User.IsInRole("Sales Executive")) q = q.Where(x => x.CreatedBy == User.Identity!.Name); return Ok(await q.OrderByDescending(x => x.Id).ToListAsync()); }
    [HttpGet("{id:int}")] public async Task<IActionResult> Get(int id) { var item = await db.Customers.FindAsync(id); if (item is null) return NotFound(); if (User.IsInRole("Sales Executive") && item.CreatedBy != User.Identity!.Name) return Forbid(); return Ok(item); }
    [HttpPost] public async Task<IActionResult> Create(Customer item) { if (!ModelState.IsValid) return ValidationProblem(ModelState); if (await db.Customers.AnyAsync(x => x.Email == item.Email || x.Phone == item.Phone)) return Conflict(new { message = "A customer with this email or phone already exists." }); item.CreatedBy = User.Identity?.Name; item.CreatedDate = DateTime.UtcNow; db.Customers.Add(item); await db.SaveChangesAsync(); await AuditService.WriteAsync(db, User, HttpContext, "Created", "Customer", item.Name); return CreatedAtAction(nameof(Get), new { id = item.Id }, item); }
    [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, Customer input) { if (id != input.Id) return BadRequest(new { message = "Route ID and record ID must match." }); if (!ModelState.IsValid) return ValidationProblem(ModelState); var item = await db.Customers.FindAsync(id); if (item is null) return NotFound(); if (User.IsInRole("Sales Executive") && item.CreatedBy != User.Identity!.Name) return Forbid(); if (await db.Customers.AnyAsync(x => x.Id != id && (x.Email == input.Email || x.Phone == input.Phone))) return Conflict(new { message = "Email or phone already belongs to another customer." }); item.Name=input.Name; item.Email=input.Email; item.Phone=input.Phone; item.Company=input.Company; item.City=input.City; item.Status=input.Status; await db.SaveChangesAsync(); await AuditService.WriteAsync(db, User, HttpContext, "Updated", "Customer", item.Name); return Ok(item); }
    [Authorize(Roles="Admin,Manager"), HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id) { var item = await db.Customers.FindAsync(id); if (item is null) return NotFound(); item.Status = "Inactive"; await db.SaveChangesAsync(); await AuditService.WriteAsync(db, User, HttpContext, "Deactivated", "Customer", item.Name); return NoContent(); }
}
