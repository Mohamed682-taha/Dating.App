using System.Security.Cryptography;
using System.Text;
using Dating.API.DTO;
using Dating.Data.Entities;
using Dating.Repository.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dating.API.Controllers;

public class AccountController(DatingDbContext context) : BaseApiController
{
    [HttpPost("Register")] // POST : /api/Account/Register
    public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
    {
        using var hmac = new HMACSHA512();
        if (await UserExists(registerDto.UserName)) return BadRequest("UserName Already Exists");
        var user = new AppUser()
        {
            UserName = registerDto.UserName,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    [HttpPost("Login")] // POST : /api/Account/Login
    public async Task<ActionResult<AppUser>> Login(LoginDto loginDto)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == loginDto.UserName.ToLower());
        if (user is null) return Unauthorized("Invalid UserName");

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        for (var i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
        }

        return user;
    }

    private async Task<bool> UserExists(string userName)
    {
        return await context.Users.AnyAsync(u => u.UserName.ToLower() == userName.ToLower());
    }
}