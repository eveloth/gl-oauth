using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OauthShowcase.Contracts;
using OauthShowcase.Identity;
using OauthShowcase.Services;

namespace OauthShowcase.Controllers;

[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserManagement _userManagement;
    private readonly IMapper _mapper;
    private readonly IValidator<ChangeAvatarRequest> _validator;

    public UsersController(
        IUserManagement userManagement,
        IValidator<ChangeAvatarRequest> validator,
        IMapper mapper
    )
    {
        _userManagement = userManagement;
        _validator = validator;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers(CancellationToken ct)
    {
        var users = await _userManagement.GetAll(ct);
        return Ok(_mapper.Map<List<UserinfoResponse>>(users));
    }

    [Authorize]
    [Route("avatar")]
    [HttpPost]
    public async Task<IActionResult> ChangeAvatar(
        [FromForm] ChangeAvatarRequest request,
        CancellationToken ct
    )
    {
        var userId = int.Parse(User.Claims.Single(x => x.Type == Claims.Subject).Value);

        await _validator.ValidateAndThrowAsync(request, ct);

        await _userManagement.ChangeAvatar(userId, request.Avatar, ct);
        return Ok();
    }

    [Authorize]
    [Route("avatar")]
    [HttpDelete]
    public async Task<IActionResult> DeleteAvatar(CancellationToken ct)
    {
        var userId = int.Parse(User.Claims.Single(x => x.Type == Claims.Subject).Value);

        await _userManagement.DeleteAvatar(userId, ct);
        return Ok();
    }

    [Authorize]
    [HttpDelete]
    [Route("purge")]
    public async Task<IActionResult> Purge(CancellationToken ct)
    {
        var userId = int.Parse(User.Claims.Single(x => x.Type == Claims.Subject).Value);
        await _userManagement.Delete(userId, ct);
        await HttpContext.SignOutAsync();
        return Ok();
    }
}