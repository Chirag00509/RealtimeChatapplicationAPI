using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Modal
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string ReceiverId { get; set; }
        public string content { get; set; }
    }
}
