using AcxiomCRM.Backend.Data; using AcxiomCRM.Backend.Models; using AcxiomCRM.Backend.Services; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore;
namespace AcxiomCRM.Backend.Controllers;
[ApiController,Route("api/activities"),Authorize]
public class ActivitiesController(CrmDbContext db):ControllerBase
{
 [HttpGet] public async Task<IActionResult> Get(){var q=db.Activities.AsQueryable();if(User.IsInRole("Sales Executive"))q=q.Where(x=>x.Assigned==User.Identity!.Name);return Ok(await q.OrderByDescending(x=>x.Date).ToListAsync());}
 [HttpPost] public async Task<IActionResult> Create(Activity x){if(!ModelState.IsValid)return ValidationProblem(ModelState);db.Activities.Add(x);await db.SaveChangesAsync();await AuditService.WriteAsync(db,User,HttpContext,"Created","Activity",x.Subject);return Created("/api/activities/"+x.Id,x);}
 [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id,Activity input){if(id!=input.Id)return BadRequest();if(!ModelState.IsValid)return ValidationProblem(ModelState);var x=await db.Activities.FindAsync(id);if(x is null)return NotFound();db.Entry(x).CurrentValues.SetValues(input);await db.SaveChangesAsync();await AuditService.WriteAsync(db,User,HttpContext,"Updated","Activity",x.Subject);return Ok(x);}
}
