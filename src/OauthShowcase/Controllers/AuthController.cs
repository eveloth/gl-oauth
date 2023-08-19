using AspNet.Security.OAuth.GitLab;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OauthShowcase.Contracts;
using OauthShowcase.Identity;
using OauthShowcase.Services;

namespace OauthShowcase.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserManagement _userManagement;
    private readonly IMapper _mapper;

    public AuthController(IUserManagement userManagement, IMapper mapper)
    {
        _userManagement = userManagement;
        _mapper = mapper;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
    {
        var newUser = new User(request.Email, request.FirstName, request.LastName);
        await _userManagement.Create(newUser, request.Password, ct);
        return Ok();
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        var existingUser = await _userManagement.Login(request.Email, request.Password, ct);

        if (existingUser is null)
        {
            return Unauthorized();
        }

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            existingUser.ToPrincipal()
        );

        return Ok();
    }

    [Authorize]
    [HttpPost]
    [Route("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        await HttpContext.SignOutAsync();
        return Ok();
    }

    [Authorize]
    [HttpGet]
    [Route("info")]
    public async Task<IActionResult> Info(CancellationToken ct)
    {
        var userId = int.Parse(User.Claims.Single(x => x.Type == Claims.Subject).Value);
        var existingUser = await _userManagement.Get(userId, ct);

        return Ok(_mapper.Map<UserinfoResponse>(existingUser!));
    }

    [Authorize]
    [HttpPost]
    [Route("purge")]
    public async Task<IActionResult> Purge(CancellationToken ct)
    {
        var userId = int.Parse(User.Claims.Single(x => x.Type == Claims.Subject).Value);
        await _userManagement.Delete(userId, ct);
        await HttpContext.SignOutAsync();
        return Ok();
    }

    [HttpGet]
    [Route("users")]
    public async Task<IActionResult> GetUsers(CancellationToken ct)
    {
        var users = await _userManagement.GetAll(ct);
        return Ok(_mapper.Map<List<UserinfoResponse>>(users));
    }

    [HttpGet]
    [Route("login-gitlab")]
    public IActionResult LoginGitLab()
    {
        return Challenge(
            new AuthenticationProperties { RedirectUri = "/" },
            GitLabAuthenticationDefaults.AuthenticationScheme
        );
    }
}