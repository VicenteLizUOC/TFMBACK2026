using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TFGBack._02_Domain.ServiceLibrary.Contracts.Contracts;
using TFGBack._02_Domain.ServiceLibrary.Contracts.Models;

namespace TFGBack._01_Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [Authorize]
        [HttpGet("/board/{boardId}/comments")]
        public ActionResult<List<BoardCommentDto>> GetComments(int boardId)
        {
            var response = _commentService.getComments(boardId);
            if (response.Count == 0)
                return NoContent();
            return Ok(response);
        }

        [Authorize]
        [HttpPost("/board/{boardId}/comments")]
        public ActionResult AddComment(int boardId, [FromBody] string content)
        {
            int userId = int.Parse(HttpContext.User.Identity!.Name!);
            var response = _commentService.addComment(boardId, userId, content);
            if (response)
                return Ok(new { message = "Comment added" });
            return BadRequest("Error");
        }

        [Authorize]
        [HttpDelete("/board/comments/{commentId}")]
        public ActionResult DeleteComment(int commentId)
        {
            int userId = int.Parse(HttpContext.User.Identity!.Name!);
            var response = _commentService.deleteComment(commentId, userId);
            if (response)
                return Ok(new { message = "Comment deleted" });
            return BadRequest("Error");
        }
    }
}
