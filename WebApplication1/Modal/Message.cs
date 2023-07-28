using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Modal
{
    public class Message
    {
        public int Id { get; set; }

        public string SenderId  { get; set; }
        public IdentityUser Sender { get; set; }
        public string ReceiverId { get; set;}
        public IdentityUser Receiver { get; set; }
        public string content { get; set; }

        public DateTime Timestemp { get; set; }
    }
}
