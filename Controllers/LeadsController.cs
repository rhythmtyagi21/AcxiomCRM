using AcxiomCRM.Backend.Data; using AcxiomCRM.Backend.Models; using AcxiomCRM.Backend.Services; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore;
namespace AcxiomCRM.Backend.Controllers;
[ApiController, Route("api/leads"), Authorize]
public class LeadsController(CrmDbContext db) : ControllerBase
{
 [HttpGet] public async Task<IActionResult> Get([FromQuery]string? search) { var q=db.Leads.AsQueryable(); if(!string.IsNullOrWhiteSpace(search)) q=q.Where(x=>x.Name.Contains(search)||x.Company.Contains(search)||x.Status.Contains(search)); if(User.IsInRole("Sales Executive")) q=q.Where(x=>x.Assigned==User.Identity!.Name); return Ok(await q.OrderByDescending(x=>x.Id).ToListAsync()); }
 [HttpGet("{id:int}")] public async Task<IActionResult> Get(int id) { var x=await db.Leads.FindAsync(id); return x is null?NotFound():Ok(x); }
 [HttpPost] public async Task<IActionResult> Create(Lead x) { if(!ModelState.IsValid)return ValidationProblem(ModelState); if(x.Value<0)return BadRequest(new{message="Expected value cannot be negative."}); if(x.Status is not ("New" or "Contacted" or "Qualified" or "Unqualified" or "Converted" or "Lost"))return BadRequest(new{message="Invalid lead status."}); db.Leads.Add(x); await db.SaveChangesAsync(); await AuditService.WriteAsync(db,User,HttpContext,"Created","Lead",x.Name); return CreatedAtAction(nameof(Get),new{id=x.Id},x); }
 [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id,Lead input) { if(id!=input.Id)return BadRequest(); if(!ModelState.IsValid)return ValidationProblem(ModelState); var x=await db.Leads.FindAsync(id); if(x is null)return NotFound(); db.Entry(x).CurrentValues.SetValues(input); await db.SaveChangesAsync(); await AuditService.WriteAsync(db,User,HttpContext,"Updated","Lead",x.Name); return Ok(x); }
 [Authorize(Roles="Admin,Manager"),HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id) { var x=await db.Leads.FindAsync(id); if(x is null)return NotFound(); db.Leads.Remove(x); await db.SaveChangesAsync(); await AuditService.WriteAsync(db,User,HttpContext,"Deleted","Lead",x.Name); return NoContent(); }
}
