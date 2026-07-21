using BE_01.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE_01.Controllers
{
    [Route("tasks")]
    [ApiController]
    public class tasksController : ControllerBase
    {
        //static list initialized with 3 tasks
        private static List<ToDoTask> Tasks = new()
        {
            new ToDoTask("Buy Milk"),
            new ToDoTask ("Watch Movie"),
            new ToDoTask ("Take Out the Trash")
        };

        //Return full list of tasks
        [HttpGet]
        public ActionResult GetAllTasks()
        {
            return Ok(Tasks);
        }

        //Return single task (by ID)
        [HttpGet("{id}")]
        public ActionResult GetTaskById(int id)
        {
            var task = Tasks.FirstOrDefault(x => x.Id == id);
            if (task == null)
            {
                return NotFound(new { error = $"Task {id} not found :(" });
            }
            return Ok(task);
        }
    }
}
