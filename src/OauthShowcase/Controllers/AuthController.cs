using System.Security.Claims;
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
    [HttpGet]
    [Route("info")]
    public async Task<IActionResult> Info(CancellationToken ct)
    {
        var userId = int.Parse(HttpContext.User.Claims.Single(x => x.Type == Claims.Subject).Value);
        var existingUser = await _userManagement.Get(userId, ct);

        return Ok(_mapper.Map<UserinfoResponse>(existingUser!));
    }

    [HttpGet]
    [Route("sync")]
    public async Task<IActionResult> GitlabSync(CancellationToken ct)
    {
        var authResult = await HttpContext.AuthenticateAsync(
            GitLabAuthenticationDefaults.AuthenticationScheme
        );

        if (!authResult.Succeeded)
        {
            var uri = Url.Action("info", "auth");

            return Challenge(
                new AuthenticationProperties { RedirectUri = uri },
                GitLabAuthenticationDefaults.AuthenticationScheme
            );
        }

        var email = authResult.Principal.Claims.Single(x => x.Type == ClaimTypes.Email).Value;
        var existingUser = await _userManagement.Get(email, ct);

        if (existingUser is null)
        {
            await _userManagement.Create(
                OauthShowcase.User.FromPrincipal(authResult.Principal),
                ct
            );
            existingUser = await _userManagement.Get(email, ct);
        }

        var externalData = ExternalData.FromAuthenticateResult(existingUser!.Id, authResult);
        await _userManagement.AddExternalData(existingUser.Id, externalData, ct);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            existingUser.ToPrincipal()
        );

        return Redirect("~/");
    }

    [HttpGet]
    [Route("login-gitlab")]
    public IActionResult LoginGitLab()
    {
        var uri = Url.Action("GitlabSync", "auth");

        return Challenge(
            new AuthenticationProperties { RedirectUri = uri },
            GitLabAuthenticationDefaults.AuthenticationScheme
        );
    }

    [HttpGet]
    [Route("users")]
    public async Task<IActionResult> GetUsers(CancellationToken ct)
    {
        var users = await _userManagement.GetAll(ct);
        return Ok(_mapper.Map<List<UserinfoResponse>>(users));
    }
}