namespace Cahut_Backend.Models
{
    public class ChatMessage
    {
        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
        public string MsgContent { get; set; }
        public Guid ChatId { get; set; }
        public DateTime TimeSend { get; set; }

        public Chat Chat { get; set; }
    }
}
