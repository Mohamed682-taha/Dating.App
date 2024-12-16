using System.Security.Claims;
using Dating.API.Extensions;
using Dating.Data.IRepositories;
using Dating.Shared;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dating.API.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var contextResult = await next();
        if (context.HttpContext.User.Identity?.IsAuthenticated != true) return;
        var userId = contextResult.HttpContext.User.GetUserId();
        var userRepo = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
        var user = await userRepo.GetUserAsync(userId);
        user!.LastActive = DateTime.UtcNow;
        await userRepo.SaveAllChangesAsync();
    }
}