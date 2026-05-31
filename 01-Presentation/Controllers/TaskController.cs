using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TFGBack._02_Domain.ServiceLibrary.Contracts.Contracts;
using TFGBack._02_Domain.ServiceLibrary.Contracts.Models;

namespace TFGBack._01_Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [Authorize]
        [HttpGet("/boards/{boardId}/tasks")]
        public ActionResult<TaskDto> GetTaskFromBoard(int userId, int boardId)
        {
            string userIdValidated = HttpContext.User.Identity.Name;
            if (userId == Int32.Parse(userIdValidated))
            {
                var response = _taskService.getTaskFromBoard(userId, boardId);
                if (response.Count > 0)
                {
                    return Ok(response);
                }
                else
                {
                    return NoContent();
                }
            }
            else
            {
                return BadRequest("Error");
            }
        }

        [Authorize]
        [HttpPost("/user/{userId}/board/{boardId}/addTask")]
        public IActionResult AddTask([FromBody] TaskDto task, int userId, int boardId)
        {
            string userIdValidated = HttpContext.User.Identity.Name;
            if (userIdValidated == null || userId == Int32.Parse(userIdValidated))
            {
                var response = _taskService.addTask(task, boardId, userId);
                if (response)
                {
                    return Ok(new { message = "Task created" });
                }
                else
                {
                    return BadRequest("Error");
                }
            }
            else
            {
                return BadRequest("Error");
            }
        }

        [Authorize]
        [HttpPost("/user/{userId}/board/{boardId}/task/{taskId}/update")]
        public IActionResult UpdateTask([FromBody] TaskDto task, int userId, int boardId, int taskId)
        {
            string userIdValidated = HttpContext.User.Identity.Name;
            if (userIdValidated == null || userId == Int32.Parse(userIdValidated))
            {
                var response = _taskService.modifyTask(userId, boardId, taskId, task);
                if (response)
                {
                    return Ok(new { message = "Task updated" });
                }
                else
                {
                    return BadRequest("Error");
                }
            }
            else
            {
                return BadRequest("Error");
            }
        }

        [Authorize]
        [HttpDelete("/user/{userId}/board/{boardId}/task/{taskId}/delete")]
        public IActionResult RemoveTask(int userId, int boardId, int taskId)
        {
            string userIdValidated = HttpContext.User.Identity.Name;
            if (userIdValidated == null || userId == Int32.Parse(userIdValidated))
            {
                var response = _taskService.removeTask(userId, boardId, taskId);
                if (response)
                {
                    return Ok(new { message = "Task removed" });
                }
                else
                {
                    return BadRequest("Error");
                }
            }
            else
            {
                return BadRequest("Error");
            }
        }

        [Authorize]
        [HttpGet("/user/{userId}/board/{boardId}/task/{taskId}/help")]
        public async Task<IActionResult> GetTaskHelp(int userId, int boardId, int taskId)
        {
            int userIdValidated = Int32.Parse(HttpContext.User.Identity.Name);
            if (userId != userIdValidated)
                return BadRequest("Error");

            var result = await _taskService.getTaskHelp(userId, taskId, boardId);

            if (result == null)
                return StatusCode(403, new { message = "Esta funcionalidad es exclusiva para usuarios pro." });

            return Ok(new { help = result });
        }


    }
}
