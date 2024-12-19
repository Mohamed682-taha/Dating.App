namespace Dating.API.DTO;

public class CreateMessageDto
{
    public required string RecipientUserName { get; set; }
    public required string Content { get; set; }
}