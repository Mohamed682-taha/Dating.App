using Dating.API.Errors;
using Dating.API.Extensions;
using Dating.Data.Entities;
using Dating.Data.IRepositories;
using Dating.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dating.API.Controllers;

[Authorize]
public class LikesController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpPost("{targetUserId:int}")] // POST : /api/Likes/6
    public async Task<ActionResult> ToggleLikeEndpoint(int targetUserId)
    {
        var sourceUserId = User.GetUserId();
        if (sourceUserId == targetUserId) return BadRequest(new ApiResponse(400, "You cannot like yourself"));

        var existingLike = await unitOfWork.LikesRepository.GetUserLike(sourceUserId, targetUserId);
        if (existingLike is null)
        {
            var like = new UserLike()
            {
                SourceUserId = sourceUserId,
                TargetUserId = targetUserId
            };
            unitOfWork.LikesRepository.AddLike(like);
        }
        else
        {
            unitOfWork.LikesRepository.DeleteLike(existingLike);
        }

        return await unitOfWork.Complete() ? Ok() : BadRequest(new ApiResponse(400, "Failed to add like"));
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
    {
        var userId = User.GetUserId();
        return Ok(await unitOfWork.LikesRepository.GetCurrentUserLikeIds(userId));
    }

    [HttpGet] // GET : /api/Likes?predicate=liked
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
    {
        likesParams.UserId = User.GetUserId();
        var users = await unitOfWork.LikesRepository.GetUserLikes(likesParams);
        Response.ApplyPagination(users);
        return Ok(users);
    }
}