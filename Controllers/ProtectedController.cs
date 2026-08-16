using Microsoft.AspNetCore.Mvc;

namespace BE_01.Controllers
{
    [ApiController]
    public class ProtectedController : ControllerBase
    {
        [HttpGet("protected/profile")]
        public ActionResult GetProfile()
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

            return Ok(new { message = "Token present (not yet verified)", token });
        }
    }
}