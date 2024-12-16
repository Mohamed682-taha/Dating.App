using System.Security.Claims;
using AutoMapper;
using Dating.API.DTO;
using Dating.API.Errors;
using Dating.API.Extensions;
using Dating.Data.Entities;
using Dating.Data.IRepositories;
using Dating.Data.IServices;
using Dating.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dating.API.Controllers;

[Authorize]
public class UsersController(
    IUserRepository userRepository,
    IMapper mapper,
    IPhotoService photoService
) : BaseApiController
{
    [HttpGet] // GET : /api/Users
    public async Task<ActionResult<PageList<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
    {
        var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
        userParams.CurrentUserName = username;
        var users = await userRepository.GetMembersAsync(userParams);
        Response.ApplyPagination(users);
        return Ok(users);
    }

    [HttpGet("{username}")] // GET : /api/Users/lisa
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var user = await userRepository.GetMemberByUserNameAsync(username);
        if (user is null) return NotFound(new ApiResponse(404));
        return Ok(user);
    }

    [HttpPut] // PUT : /api/Users
    public async Task<ActionResult> UpdateUser(MemberUpdateDto dto)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var user = await userRepository.GetUserByUserName(username);
        mapper.Map(dto, user);
        if (await userRepository.SaveAllChangesAsync()) return NoContent();
        return BadRequest("Failed to update user");
    }

    [HttpPost("addPhoto")] // POST : /api/Users/addPhoto
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await userRepository.GetUserByUserName(userName!);
        var result = await photoService.AddPhotoAsync(file);
        if (result.Error != null) return BadRequest(new ApiResponse(400, result.Error.Message));

        var photo = new Photo()
        {
            Url = result.Url.AbsoluteUri,
            PublicId = result.PublicId
        };
        user!.Photos.Add(photo);
        if (await userRepository.SaveAllChangesAsync())
            return CreatedAtAction(nameof(GetUser), new { username = user.UserName }, mapper.Map<PhotoDto>(photo));

        return BadRequest(new ApiResponse(400, "Failed to add photo"));
    }

    [HttpPut("setMainPhoto/{photoId:int}")] // PUT : /api/Users/setMainPhoto/2
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await userRepository.GetUserByUserName(userName!);
        var photo = user!.Photos.FirstOrDefault(p => p.Id == photoId);
        // i have checked that id is correct and photo is not main
        if (photo is null || photo.IsMain) return BadRequest(new ApiResponse(400, "cannot use this as main photo"));
        var currentMain = user.Photos.FirstOrDefault(p => p.IsMain);
        // check if he have a main photo , if he have a photo i will set it as is main = false
        if (currentMain is not null) currentMain.IsMain = false;
        // make photo that he entered main
        photo.IsMain = true;
        if (await userRepository.SaveAllChangesAsync()) return NoContent();
        return BadRequest(new ApiResponse(400, "problem setting main photo"));
    }

    [HttpDelete("deletePhoto/{photoId:int}")] // DELETE : /api/Users/deletePhoto/3
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await userRepository.GetUserByUserName(userName!);
        var photo = user!.Photos.FirstOrDefault(p => p.Id == photoId);
        if (photo is null || photo.IsMain) return BadRequest(new ApiResponse(400, "cannot delete this photo"));

        if (photo.PublicId is not null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error is not null) return BadRequest(new ApiResponse(400, result.Error.Message));
        }

        user.Photos.Remove(photo);
        if (await userRepository.SaveAllChangesAsync()) return Ok();
        return BadRequest(new ApiResponse(400, "problem deleting photo"));
    }
}