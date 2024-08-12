public class MessageDto
{
    public Guid MessageId { get; set; }
    public string SenderId { get; set; }
    public string ReceiverId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedTime { get; set; }
    public bool IsRead { get; set; }
}
