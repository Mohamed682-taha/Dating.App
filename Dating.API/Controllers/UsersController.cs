using AutoMapper;
using Dating.API.DTO;
using Dating.API.Errors;
using Dating.Data.Entities;
using Dating.Data.IRepositories;
using Dating.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dating.API.Controllers;

[Authorize]
public class UsersController(IUserRepository userRepository, IMapper mapper) : BaseApiController
{
    [HttpGet] // GET : /api/Users
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var users = await userRepository.GetMembersAsync();
        return Ok(users);
    }

    [HttpGet("{username}")] // GET : /api/Users/lisa
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var user = await userRepository.GetMemberByUserNameAsync(username);
        if (user is null) return NotFound(new ApiResponse(404));
        return Ok(user);
    }
}