using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Modal;

namespace WebApplication1.Data
{
    public class ChatContext : IdentityDbContext
    {
        public ChatContext(DbContextOptions<ChatContext> options) : base(options) 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Message>().ToTable("Message");
            modelBuilder.Entity<Logs>().ToTable("Logs");

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Message> Message { get; set; }
        public DbSet<Logs> Logs { get; set; }
    }
}
