using ePermits.Models;

namespace ePermitsApp.Entities
{
    public class MessageRecipientState
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public int RecipientUserId { get; set; }
        public string RecipientRole { get; set; } = string.Empty;
        public string SenderType { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }

        public Message? Message { get; set; }
        public User? RecipientUser { get; set; }
    }
}
