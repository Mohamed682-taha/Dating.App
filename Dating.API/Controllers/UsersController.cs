using Dating.Data.Entities;
using Dating.Repository.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dating.API.Controllers;

public class UsersController(DatingDbContext context) : BaseApiController
{

    [HttpGet] // GET : /api/Users
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        var users = await context.Users.ToListAsync();
        return Ok(users);
    }

    [HttpGet("{id:int}")] // GET : /api/Users/1
    public async Task<ActionResult<AppUser>> GetUser(int id)
    {
        var user = await context.Users.FindAsync(id);
        return user is null ? NotFound() : Ok(user);
    }
    
}