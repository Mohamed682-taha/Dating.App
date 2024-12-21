using Dating.API.Errors;
using Dating.API.Extensions;
using Dating.Data.Entities;
using Dating.Data.IRepositories;
using Dating.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dating.API.Controllers;

[Authorize]
public class LikesController(ILikesRepository likeRepo) : BaseApiController
{
    [HttpPost("{targetUserId:int}")] // POST : /api/Likes/6
    public async Task<ActionResult> ToggleLikeEndpoint(int targetUserId)
    {
        var sourceUserId = User.GetUserId();
        if (sourceUserId == targetUserId) return BadRequest(new ApiResponse(400, "You cannot like yourself"));

        var existingLike = await likeRepo.GetUserLike(sourceUserId, targetUserId);
        if (existingLike is null)
        {
            var like = new UserLike()
            {
                SourceUserId = sourceUserId,
                TargetUserId = targetUserId
            };
            likeRepo.AddLike(like);
        }
        else
        {
            likeRepo.DeleteLike(existingLike);
        }

        return await likeRepo.SaveAllChanges() ? Ok() : BadRequest(new ApiResponse(400, "Failed to add like"));
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
    {
        var userId = User.GetUserId();
        return Ok(await likeRepo.GetCurrentUserLikeIds(userId));
    }

    [HttpGet] // GET : /api/Likes?predicate=liked
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
    {
        likesParams.UserId = User.GetUserId();
        var users = await likeRepo.GetUserLikes(likesParams);
        Response.ApplyPagination(users);
        return Ok(users);
    }
}