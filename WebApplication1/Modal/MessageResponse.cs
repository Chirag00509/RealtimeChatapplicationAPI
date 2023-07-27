namespace WebApplication1.Modal
{
    public class MessageResponse
    {
        public int MessageId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime Timestemp { get; set; }

    }
}
