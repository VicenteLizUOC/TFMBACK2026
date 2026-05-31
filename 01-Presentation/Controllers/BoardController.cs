using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TFGBack._02_Domain.ServiceLibrary.Contracts.Contracts;
using TFGBack._02_Domain.ServiceLibrary.Contracts.Models;

namespace TFGBack._01_Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardController : ControllerBase
    {
        private readonly IBoardService _boardService;

        public BoardController(IBoardService boardService)
        {
            _boardService = boardService;
        }

        [HttpGet("/boards/community")]
        public ActionResult<PagedBoardsDto> GetCommunityBoards(int id, int page = 1)
        {
            var response = _boardService.getCommunityBoards(id, page);
            return Ok(response);
        }


        [Authorize]
        [HttpPost("/user/{userId}/addboard")]
        public IActionResult AddBoard([FromBody] BoardDto board, int userId)
        {
            string userIdValidated = HttpContext.User.Identity.Name;
            if (userIdValidated == null || userId == Int32.Parse(userIdValidated))
            {
                var newBoardId = _boardService.addBoard(board, userId);
                if (newBoardId > 0)
                {
                    return Ok(new { message = "Board created", id = newBoardId });
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
        [HttpPost("/user/{userId}/follow/{boardId}")]
        public IActionResult FollowBoard(int userId, int boardId)
        {
            string userIdValidated = HttpContext.User.Identity.Name;
            if (userIdValidated == null || userId == Int32.Parse(userIdValidated))
            {
                var response = _boardService.followBoard(userId, boardId);
                if (response)
                {
                    return Ok(new { message = "Done" });
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
        [HttpGet("/boards/{boardId}")]
        public ActionResult<BoardDto> GetYourBoard(int userId, int boardId)
        {
            string userIdValidated = HttpContext.User.Identity.Name;
            if (userId == Int32.Parse(userIdValidated))
            {
                var response = _boardService.getSingleBoard(userId, boardId);
                if (response.Id != null)
                {
                    return Ok(response);
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
        [HttpPost("/boards/{boardId}/update")]
        public ActionResult modifyBoard(int userId, int boardId, BoardDto boardDto)
        {
            string userIdValidated = HttpContext.User.Identity.Name;
            if (userId == Int32.Parse(userIdValidated))
            {
                var response = _boardService.modifyBoard(userId, boardId, boardDto);
                if (response)
                {
                    return Ok(response);
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
        [HttpGet("/boards/{userId}/all")]
        public ActionResult<List<BoardDto>> GetMyBoards(int userId)
        {
            string userIdValidated = HttpContext.User.Identity.Name;
            if (userId == Int32.Parse(userIdValidated))
            {
                var response = _boardService.getMyyBoards(userId);
                return Ok(response);
            }
            else
            {
                return BadRequest("Error");
            }

        }

        [Authorize]
        [HttpPost("/user/{userId}/board/{boardId}/copy")]
        public ActionResult CopyBoard(int userId, int boardId)
        {
            int userIdValidated = Int32.Parse(HttpContext.User.Identity.Name);
            if (userId != userIdValidated)
                return BadRequest("Error");

            var result = _boardService.copyBoard(userId, boardId);
            if (result.newBoardId > 0)
                return Ok(new { message = "Board copied", id = result.newBoardId, originalBoardId = result.originalBoardId });

            return BadRequest("Error");
        }

        [Authorize]
        [HttpDelete("/boards/{boardId}/remove")]
        public ActionResult RemoveBoard(int userId, int boardId)
        {
            string userIdValidated = HttpContext.User.Identity.Name;
            if (userId == Int32.Parse(userIdValidated))
            {
                var response = _boardService.removeBoard(userId, boardId);
                if (response)
                {
                    return Ok(response);
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
        [HttpGet("/board/public/{boardId}")]
        public ActionResult<PublicBoardDto> GetPublicBoard(int boardId)
        {
            var response = _boardService.getPublicBoard(boardId);
            if (response.Id == 0)
            {
                return NotFound("Board not found or not public");
            }
            return Ok(response);
        }

        [Authorize]
        [HttpGet("/user/{userId}/boards/{boardId}/refresh")]
        public ActionResult RefreshBoard(int userId, int boardId)
        {
            string userIdValidated = HttpContext.User.Identity.Name;
            if (userId == Int32.Parse(userIdValidated))
            {
                var (endDate, streak, bestStreak) = _boardService.refreshBoard(userId, boardId);
                if (endDate.HasValue)
                {
                    return Ok(new { endDate = endDate.Value, streak, bestStreak });
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
        [HttpGet("/user/{userId}/boards/export")]
        public ActionResult ExportData(int userId)
        {
            int userIdValidated = Int32.Parse(HttpContext.User.Identity.Name);
            if (userId != userIdValidated)
                return BadRequest("Error");

            var data = _boardService.getExportData(userId);
            return Ok(data);
        }
    }
}
