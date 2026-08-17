using BE_01.Data;
using BE_01.Models;
using BE_01.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE_01.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SupabaseAuthService _authService;

        public AuthController(SupabaseAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signup")]
        public async Task<ActionResult> SignUp(SignupRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { error = "Email and password are required." });
            }

            var (success, statusCode, body) = await _authService.SignUp(request.Email, request.Password);

            if (!success)
            {
                return StatusCode(statusCode, body);
            }

            return StatusCode(201, body);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { error = "Email and password are required." });
            }

            var (success, statusCode, body) = await _authService.Login(request.Email, request.Password);

            if (!success)
            {
                return StatusCode(401, new { error = "Invalid login credentials" });
            }

            return StatusCode(200, body);
        }

        [Authorize(AuthenticationSchemes = "Supabase")]
        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            var token = User.FindFirst("access_token")?.Value;
            await _authService.SignOut(token);
            return NoContent();
        }
    }
}