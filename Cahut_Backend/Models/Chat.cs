namespace Cahut_Backend.Models
{
    public class Chat
    {
        public Guid PresentationId { get; set; }
        public Guid ChatId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int NumOfMessage { get; set; }

        public Presentation Presentation { get; set; }
        public List<ChatMessage> ChatMessages { get; set; }
    }
}
