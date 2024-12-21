using System.Security.Cryptography;
using AutoMapper;
using Dating.API.DTO;
using Dating.API.Errors;
using Dating.Data.Entities;
using Dating.Data.IServices;
using Dating.Repository.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dating.API.Controllers;

public class AccountController(
    UserManager<AppUser> userManager,
    ITokenService tokenService,
    IMapper mapper
) : BaseApiController
{
    
    [HttpPost("Register")] // POST : /api/Account/Register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.UserName)) return BadRequest("UserName Already Exists");
        
        var user = mapper.Map<AppUser>(registerDto);
        user.UserName = registerDto.UserName.ToLower();

        var result = await userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);
        
        return new UserDto()
        {
            UserName = user.UserName,
            Token = await tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    [HttpPost("Login")] // POST : /api/Account/Login
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await userManager.Users.Include(u => u.Photos)
            .FirstOrDefaultAsync(u => u.NormalizedUserName == loginDto.UserName.ToUpper());

        if (user?.UserName is null) return Unauthorized("Invalid UserName");

        var result = await userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!result) return Unauthorized(new ApiResponse(401, "Invalid Password"));
        
        return new UserDto()
        {
            UserName = user.UserName,
            Token = await tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(u => u.IsMain)?.Url,
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    private async Task<bool> UserExists(string userName)
    {
        return await userManager.Users.AnyAsync(u => u.NormalizedUserName == userName.ToUpper());
    }
}