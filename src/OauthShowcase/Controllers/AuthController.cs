using AspNet.Security.OAuth.GitLab;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace OauthShowcase.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpGet]
        [Route("info")]
        public async Task<IActionResult> Info()
        {
            var claims = HttpContext.User.Claims.Select(x => new { x.Type, x.Value });

            return Ok(claims);
        }

        [HttpGet]
        [Route("login-gitlab")]
        public async Task<IActionResult> LoginGitLab()
        {
            var uri = Url.Action("info", "auth");

            return Challenge(
                new AuthenticationProperties { RedirectUri = uri },
                GitLabAuthenticationDefaults.AuthenticationScheme
            );
        }
    }
}