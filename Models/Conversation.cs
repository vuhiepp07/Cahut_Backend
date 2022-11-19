namespace Cahut_Backend.Models
{
    public class Conversation
    {
        public Guid ConversationId { get; set; }    
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Message { get; set; }
        public DateTime SendTime { get; set; }
    }
}
