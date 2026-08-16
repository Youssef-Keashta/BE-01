using BE_01.Security;
using Microsoft.AspNetCore.Mvc;

namespace BE_01.Controllers
{
    [ApiController]
    public class ProtectedController : ControllerBase
    {
        private readonly SupabaseAuthService _authService;

        public ProtectedController(SupabaseAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("protected/profile")]
        public async Task<ActionResult> GetProfile()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { error = "Access token required" });
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { error = "Access token required" });
            }

            var (valid, body) = await _authService.GetUser(token);

            if (!valid)
            {
                return Unauthorized(new { error = "Invalid or expired token" });
            }

            return Ok(body);
        }
    }
}