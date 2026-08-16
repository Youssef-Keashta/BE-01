using Microsoft.AspNetCore.Mvc;

namespace BE_01.Controllers
{
    [ApiController]
    public class PublicController : ControllerBase
    {
        [HttpGet("public/info")]
        public ActionResult GetInfo()
        {
            return Ok(new { message = "Welcome stranger! This info is public." });
        }
    }
}