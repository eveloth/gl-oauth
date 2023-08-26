using AspNet.Security.OAuth.GitLab;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OauthShowcase.Contracts;
using OauthShowcase.Domain;
using OauthShowcase.Identity;
using OauthShowcase.Options;
using OauthShowcase.Services;

namespace OauthShowcase.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthenticator _authenticator;
    private readonly IUserManagement _userManagement;
    private readonly IMapper _mapper;
    private readonly SpaOptions _spaOptions;

    public AuthController(
        IUserManagement userManagement,
        IMapper mapper,
        IOptions<SpaOptions> spaOptions,
        IAuthenticator authenticator
    )
    {
        _userManagement = userManagement;
        _mapper = mapper;
        _authenticator = authenticator;
        _spaOptions = spaOptions.Value;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
    {
        var newUser = new User(request.Email, request.FirstName, request.LastName);
        var registrationSuccessful = await _authenticator.Register(newUser, request.Password, ct);

        return registrationSuccessful ? Ok() : BadRequest();
    }

    [HttpGet]
    [Route("confirm/{id:int}")]
    public async Task<IActionResult> ConfirmRegistration(
        [FromRoute] int id,
        [FromQuery] Guid confirmationToken,
        CancellationToken ct
    )
    {
        return await _authenticator.ConfirmRegistration(id, confirmationToken, ct)
            ? Ok()
            : BadRequest();
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        var existingUser = await _userManagement.Get(request.Email, ct);

        if (existingUser is null || !existingUser.Confirmed)
        {
            return BadRequest();
        }

        var authResult = await _authenticator.SignIn(existingUser, request.Password, ct);

        return authResult ? Ok() : BadRequest();
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

    [HttpGet]
    [Route("login-gitlab")]
    public IActionResult LoginGitLab()
    {
        var redirectUri = _spaOptions.Enabled ? _spaOptions.MainPageUrl : "/";

        return Challenge(
            new AuthenticationProperties { RedirectUri = redirectUri },
            GitLabAuthenticationDefaults.AuthenticationScheme
        );
    }
}