using BE_01.Data;
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
        //private static List<ToDoTask> Tasks = new()
        //{
        //    new ToDoTask("Buy Milk"),
        //    new ToDoTask ("Watch Movie"),
        //    new ToDoTask ("Take Out the Trash")
        //};

        //Return full list of tasks
        [HttpGet]
        public ActionResult GetAllTasks()
        {
            return Ok(TaskDatabase.GetAll());
        }

        //Return single task (by ID)
        [HttpGet("{id}")]
        public ActionResult GetTaskById(int id)
        {
            var task = TaskDatabase.GetById(id);
            if (task == null)
            {
                return NotFound(new { error = $"Task {id} not found :(" });
            }
            return Ok(task);
        }

        //Create new task
        [HttpPost]
        public ActionResult AddTask(CreatedTaskRequest request)
        {

            if (request == null || string.IsNullOrEmpty(request.Title))
            {
                return BadRequest(new { error = "Title can't be empty :(" });
            }

            var newTask = TaskDatabase.Insert(request.Title);

            return CreatedAtAction(nameof(GetTaskById), new { id = newTask.Id }, newTask);
        }

        ////Update existing task
        //[HttpPut("{id}")]
        //public ActionResult UpdateTask(int id, UpdatedTaskRequest updatedTask)
        //{
        //    var task = Tasks.FirstOrDefault(task => task.Id == id);
        //    if (task == null)
        //    {
        //        return NotFound(new { error = $"Task {id} not found :(" });
        //    }
        //    if (updatedTask == null || string.IsNullOrEmpty(updatedTask.Title))
        //    {
        //        return BadRequest(new { error = "Title can't be empty :(" });
        //    }
        //    task.Title = updatedTask.Title;
        //    task.Done = updatedTask.Done;

        //    return Ok(task);
        //}

        ////Delete existing task
        //[HttpDelete("{id}")]
        //public ActionResult DeleteTask(int id)
        //{
        //    var task = Tasks.FirstOrDefault(task => task.Id == id);
        //    if (task == null)
        //    {
        //        return NotFound(new { error = $"Task {id} not found :(" });
        //    }
        //    Tasks.Remove(task);
        //    return NoContent();
        //}
    }
}
