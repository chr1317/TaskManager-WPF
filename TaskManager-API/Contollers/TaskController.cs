using System.Reflection;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager_API.Dto;
using TaskManager_API.Models;

namespace TaskManager_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskController(AppDbContext context)
        {
            _context = context;
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks(
        //    [FromQuery] int uid,
        //    [FromQuery] int? cid,
        //    [FromQuery] int? status,
        //    [FromQuery] string? priority,
        //    [FromQuery] DateTime? endDateBefore)
        //{
        //    var query = _context.Tasks.Where(t => t.UserId == uid);

        //    if (cid.HasValue)
        //        query = query.Where(t => t.CategoryId == cid);

        //    if (status.HasValue)
        //        query = query.Where(t => t.Status == status);

        //    if (!string.IsNullOrEmpty(priority))
        //        query = query.Where(t => t.Priority == priority);

        //    if (endDateBefore.HasValue)
        //        query = query.Where(t => t.EndDate <= endDateBefore);

        //    var result = await query
        //        .Select(t => new TaskDto
        //        {
        //            TID = t.Id, // <- mapowanie Id -> TID!
        //            UID = t.UserId,
        //            CID = t.CategoryId ?? 0,
        //            Title = t.Title,
        //            Description = t.Description,
        //            Priority = t.Priority,
        //            Status = t.Status,
        //            PID = t.PID,
        //            StartDate = t.StartDate,
        //            EndDate = t.EndDate
        //        })
        //        .ToListAsync();

        //    return result;
        //}

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks(
            [FromQuery] int uid,
            [FromQuery] int? cid,
            [FromQuery] int? status,
            [FromQuery] string? priority,
            [FromQuery] DateTime? endDateBefore)
        {
            var query = _context.Tasks.Where(t => t.UserId == uid);

            if (cid.HasValue)
                query = query.Where(t => t.CategoryId == cid);

            if (status.HasValue)
                query = query.Where(t => t.Status == status);

            if (!string.IsNullOrEmpty(priority) && Enum.TryParse<PriorityLevel>(priority, true, out var parsedPriority))
            {
                query = query.Where(t => t.Priority == parsedPriority);
            }

            if (endDateBefore.HasValue)
                query = query.Where(t => t.EndDate <= endDateBefore);

            var result = await query
                .Select(t => new TaskDto
                {
                    TID = t.Id, // <- mapowanie Id -> TID!
                    UID = t.UserId,
                    CID = t.CategoryId ?? 0,
                    Title = t.Title,
                    Description = t.Description,
                    Priority = t.Priority,
                    Status = t.Status,
                    PID = t.PID,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate
                })
                .ToListAsync();

            return result;
        }


        // GET: api/task/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                return NotFound();

            return task;
        }

        // POST: api/task
        [HttpPost]
        public async Task<ActionResult<TaskItem>> CreateTask(TaskDto dto)
        {
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                Status = dto.Status,
                CategoryId = dto.CID,
                UserId = dto.UID,
                PID = dto.PID,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        // PUT: api/task/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskDto dto)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                return NotFound();

            // Aktualizacja pól
            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Priority = dto.Priority;
            task.Status = dto.Status;
            task.CategoryId = dto.CID;
            task.UserId = dto.UID;
            task.PID = dto.PID;
            task.StartDate = dto.StartDate;
            task.EndDate = dto.EndDate;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Błąd aktualizacji zadania.");
            }

            return NoContent();
        }

        // DELETE: api/task/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                return NotFound();

            var subtasks = await _context.Tasks.Where(t => t.PID == id).ToListAsync();
            _context.Tasks.RemoveRange(subtasks);
            _context.Tasks.Remove(task);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
