using AcxiomCRM.Backend.Data; using AcxiomCRM.Backend.Models; using AcxiomCRM.Backend.Services; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore;
namespace AcxiomCRM.Backend.Controllers;
[ApiController,Route("api/followups"),Authorize]
public class FollowUpsController(CrmDbContext db):ControllerBase
{
 [HttpGet] public async Task<IActionResult> Get(){var q=db.FollowUps.AsQueryable();if(User.IsInRole("Sales Executive"))q=q.Where(x=>x.Assigned==User.Identity!.Name);return Ok(await q.OrderBy(x=>x.Date).ToListAsync());}
 [HttpGet("{id:int}")] public async Task<IActionResult> Get(int id){var x=await db.FollowUps.FindAsync(id);return x is null?NotFound():Ok(x);}
 [HttpPost] public async Task<IActionResult> Create(FollowUp x){if(!ModelState.IsValid)return ValidationProblem(ModelState);if(x.Status=="Planned"&&x.Date.Date<DateTime.UtcNow.Date)return BadRequest(new{message="Follow up date cannot be earlier than today."});db.FollowUps.Add(x);await db.SaveChangesAsync();await AuditService.WriteAsync(db,User,HttpContext,"Created","FollowUp",x.Subject);return CreatedAtAction(nameof(Get),new{id=x.Id},x);}
 [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id,FollowUp input){if(id!=input.Id)return BadRequest();if(!ModelState.IsValid)return ValidationProblem(ModelState);if(input.Status=="Planned"&&input.Date.Date<DateTime.UtcNow.Date)return BadRequest(new{message="Follow up date cannot be earlier than today."});var x=await db.FollowUps.FindAsync(id);if(x is null)return NotFound();db.Entry(x).CurrentValues.SetValues(input);await db.SaveChangesAsync();await AuditService.WriteAsync(db,User,HttpContext,"Updated","FollowUp",x.Subject);return Ok(x);}
 [Authorize(Roles="Admin,Manager"),HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id){var x=await db.FollowUps.FindAsync(id);if(x is null)return NotFound();db.FollowUps.Remove(x);await db.SaveChangesAsync();await AuditService.WriteAsync(db,User,HttpContext,"Deleted","FollowUp",x.Subject);return NoContent();}
}
