using AutoMapper;
using Dating.API.DTO;
using Dating.API.Errors;
using Dating.API.Extensions;
using Dating.Data.Entities;
using Dating.Data.IRepositories;
using Dating.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dating.API.Controllers;

[Authorize]
public class MessageController(
    IUnitOfWork unitOfWork,
    IUserRepository userRepo,
    IMapper mapper
) : BaseApiController
{
    [HttpPost] // POST : /api/Message
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var userName = User.GetUserName();
        if (userName == createMessageDto.RecipientUserName.ToLower())
            return BadRequest(new ApiResponse(400, "Cannot send message to yourself"));

        var sender = await userRepo.GetUserByUserName(userName);
        var recipient = await userRepo.GetUserByUserName(createMessageDto.RecipientUserName);

        if (sender is null || recipient is null || sender.UserName is null || recipient.UserName is null)
            return BadRequest(new ApiResponse(400, "Cannot send message at this time"));

        var message = new Message()
        {
            Sender = sender,
            Recipient = recipient,
            SenderUserName = sender.UserName,
            RecipientUserName = recipient.UserName,
            Content = createMessageDto.Content
        };
        unitOfWork.MessageRepository.AddMessage(message);

        return await unitOfWork.Complete()
            ? Ok(mapper.Map<MessageDto>(message))
            : BadRequest(new ApiResponse(400, "Failed to add message"));
    }

    [HttpGet] // GET : /api/Message
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.UserName = User.GetUserName();
        var messages = await unitOfWork.MessageRepository.GetMessagesForUser(messageParams);
        Response.ApplyPagination(messages);
        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {
        var currentUserName = User.GetUserName();
        return Ok(await unitOfWork.MessageRepository.GetMessageThread(currentUserName, username));
    }

    [HttpDelete] // POST : /api/Message
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUserName();
        var message = await unitOfWork.MessageRepository.GetMessage(id);
        if (message is null) return BadRequest(new ApiResponse(400, "Cannot delete message"));
        if (message.SenderUserName != username || message.RecipientUserName != username)
            return Forbid();
        if (message.SenderUserName == username) message.SenderDeleted = true;
        if (message.RecipientUserName == username) message.RecipientDeleted = true;

        if (message is { SenderDeleted: true, RecipientDeleted: true })
            unitOfWork.MessageRepository.DeleteMessage(message);
        if (await unitOfWork.Complete()) return Ok();
        return BadRequest(new ApiResponse(400, "Problem deleting message"));
    }
}