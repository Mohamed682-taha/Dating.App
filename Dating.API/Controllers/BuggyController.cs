using Dating.Data.Entities;
using Dating.Repository.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dating.API.Controllers;

public class BuggyController(DatingDbContext context) : BaseApiController
{
    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetAuthError()
    {
        return "not authorized";
    }

    [HttpGet("notFound")]
    public ActionResult<AppUser> GetNotFound()
    {
        var user = context.Users.Find(-1);
        if (user is null) return NotFound();
        return user;
    }

    [HttpGet("serverError")]
    public ActionResult<AppUser> GetServerError()
    {
        var user = context.Users.Find(-1) ?? throw new Exception("server error");
        return user;
    }

    [HttpGet("badRequest")]
    public ActionResult<string> GetBadRequest()
    {
        return BadRequest("bad request error");
    }
}