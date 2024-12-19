namespace Dating.Shared;

public class MessageDto
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public required string SenderUserName { get; set; }
    public required string SenderPhotoUrl { get; set; }
    public int RecipientId { get; set; }
    public required string RecipientUserName { get; set; }
    public required string RecipientPhotoUrl { get; set; }
    public required string Content { get; set; }
    public DateTime? DateRead { get; set; }
    public DateTime MessageSentDate { get; set; }
}