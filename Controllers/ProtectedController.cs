using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE_01.Controllers
{
    [ApiController]
    public class ProtectedController : ControllerBase
    {
        [Authorize(AuthenticationSchemes = "Supabase")]
        [HttpGet("protected/profile")]
        public ActionResult GetProfile()
        {
            var userJson = User.FindFirst("supabase_user")?.Value;
            return Ok(userJson);
        }

        [Authorize(AuthenticationSchemes = "Supabase")]
        [HttpGet("protected/dashboard")]
        public ActionResult GetDashboard()
        {
            return Ok(new { message = "This is your private dashboard." });
        }
    }
}