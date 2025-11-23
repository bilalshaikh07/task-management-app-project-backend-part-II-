using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskMgmAPI.Models;
using Task = TaskMgmAPI.Models.Task;

namespace TaskMgmAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly TaskContext _context;
        public TaskController(TaskContext db) => _context = db;


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Task>>> GetAllTasks()
        {
            var tasks = await _context.Tasks.ToListAsync();

            if (tasks == null || !tasks.Any())
                return NotFound("No tasks found.");

            return Ok(tasks);
        }
        // if we want to give specific permission to user  for their owno task to make any changes 
        //[HttpGet("{userId}")]
        //public async Task<ActionResult<IEnumerable<Task>>> GetTasksByUser(int userId)
        //{
        //    var tasks = await _context.Tasks
        //                              .Where(t => t.UserId == userId)
        //                              .ToListAsync();

        //    if (tasks == null || !tasks.Any())
        //        return NotFound($"No tasks found for userId {userId}");

        //    return Ok(tasks);
        //}


        // POST: api/tasks
        [HttpPost]
        public async Task<ActionResult<Task>> CreateTask(Task task)
        {
            if (string.IsNullOrEmpty(task.Title) || string.IsNullOrEmpty(task.Description))
                return BadRequest("Title and Description are required.");
            if (task.Description.Length > 250)
                return BadRequest("Description max 250 characters.");
            if (task.DueDate <= DateTime.Now)
                return BadRequest("DueDate must be future date.");

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return Ok( task);
        }

        // PUT: api/tasks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, Task task)
        {
            if (id != task.Id) return BadRequest();
            if (string.IsNullOrEmpty(task.Title) || string.IsNullOrEmpty(task.Description))
                return BadRequest("Title and Description are required.");
            if (task.Description.Length > 250)
                return BadRequest("Description max 250 characters.");
            if (task.DueDate <= DateTime.Now)
                return BadRequest("DueDate must be future date.");

            _context.Entry(task).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Tasks.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return Ok();
        }

        // DELETE: api/tasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return Ok();
        }


    }
}
