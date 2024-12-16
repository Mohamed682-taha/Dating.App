using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Dating.API.DTO;
using Dating.Data.Entities;
using Dating.Data.IServices;
using Dating.Repository.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dating.API.Controllers;

public class AccountController(
    DatingDbContext context,
    ITokenService tokenService,
    IMapper mapper
) : BaseApiController
{
    [HttpPost("Register")] // POST : /api/Account/Register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.UserName)) return BadRequest("UserName Already Exists");

        using var hmac = new HMACSHA512();

        var user = mapper.Map<AppUser>(registerDto);
        user.UserName = registerDto.UserName.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
        user.PasswordSalt = hmac.Key;

        context.Users.Add(user);
        await context.SaveChangesAsync();
        return new UserDto()
        {
            UserName = user.UserName,
            Token = tokenService.CreateToken(user),
            KnownAs = user.KnownAs
        };
    }

    [HttpPost("Login")] // POST : /api/Account/Login
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await context.Users.Include(u => u.Photos)
            .FirstOrDefaultAsync(u => u.UserName.ToLower() == loginDto.UserName.ToLower());
        if (user is null) return Unauthorized("Invalid UserName");

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        for (var i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
        }

        return new UserDto()
        {
            UserName = user.UserName,
            Token = tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(u => u.IsMain)?.Url,
            KnownAs = user.KnownAs
        };
    }

    private async Task<bool> UserExists(string userName)
    {
        return await context.Users.AnyAsync(u => u.UserName.ToLower() == userName.ToLower());
    }
}