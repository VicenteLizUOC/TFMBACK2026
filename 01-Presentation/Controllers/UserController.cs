using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TFGBack._02_Domain.ServiceLibrary.Contracts.Contracts;
using TFGBack._02_Domain.ServiceLibrary.Contracts.Models;

namespace TFGBack._01_Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //Add auth 
        //bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify("passs", singleUserRepository.Password);

        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody] UserDto user)
        {
            var response = await _userService.registerUser(user);
            if (response)
            {
                return Ok(new { message = "User created" });
            }
            else
            {
                return BadRequest("Error");
            }
        }

        [Authorize]
        [HttpPost("/login")]
        public ActionResult<UserDto> Login([FromBody] UserDto user)
        {
            string emailValidated = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if(emailValidated != null && emailValidated == user.Email)
            {
                var response = _userService.login(user);
                if (response.Email != null)
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
        [HttpGet("/user/{id}")]
        public ActionResult<UserDto> GetUser(int id)
        {

            string userIdValidated = HttpContext.User.Identity.Name;
            if (id == Int32.Parse(userIdValidated))
            {
                var response = _userService.getUser(id);
                if (response.IsAvailable != false)
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
        [HttpDelete("/user/delete/{id}")]
        public ActionResult RemoveUser(int id)
        {
            string specialistIdValidated = HttpContext.User.Identity.Name;
            if (id == Int32.Parse(specialistIdValidated))
            {
                var response = _userService.removeUser(id);
                if (response)
                {
                    return Ok("User removed");
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
        [HttpGet("/user/profile/{targetId}")]
        public ActionResult<UserProfileDto> GetUserProfile(int targetId)
        {
            int requestingUserId = int.Parse(HttpContext.User.Identity!.Name!);
            var response = _userService.getUserProfile(targetId, requestingUserId);
            if (response.Id == 0)
            {
                return NotFound("User not found");
            }
            return Ok(response);
        }

        [Authorize]
        [HttpPost("/user/{userId}/followuser/{targetId}")]
        public ActionResult ToggleFollow(int userId, int targetId)
        {
            string userIdValidated = HttpContext.User.Identity!.Name!;
            if (userId != int.Parse(userIdValidated))
            {
                return BadRequest("Error");
            }

            if (userId == targetId)
            {
                return BadRequest("Cannot follow yourself");
            }

            var response = _userService.toggleUserFollow(userId, targetId);
            if (response)
            {
                return Ok(new { message = "Done" });
            }
            return BadRequest("Error");
        }

        [Authorize]
        [HttpPost("/modify/{id}")]
        public async Task<IActionResult> ModifyUser(int id, [FromForm] UserDto userDto)
        {
            string specialistIdValidated = HttpContext.User.Identity.Name;
            if (id == Int32.Parse(specialistIdValidated))
            {
                var response = await _userService.modifyUser(id, userDto);
                if (response)
                {
                    return Ok(new { message = "User modified" });
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
    }
}
