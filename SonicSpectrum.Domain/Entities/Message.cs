namespace SonicSpectrum.Domain.Entities
{
    public  class Message
    {
        public Guid MessageId { get; set; } = Guid.NewGuid();
        public string SenderId { get; set; } = null!;
        public string ReceiverId { get; set; } = null!;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;

        public virtual  User? Sender { get; set; }
        public virtual User? Receiver { get; set; }
    }
}
