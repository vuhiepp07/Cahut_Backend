using Cahut_Backend.Models;

namespace Cahut_Backend.Repository
{
    public class ChatRepository : BaseRepository
    {
        public ChatRepository(AppDbContext context) : base(context)
        {

        }
        public int createNewChat(Guid presentationId)
        {
            Chat newChat = new Chat
            {
                ChatId = Guid.NewGuid(),
                NumOfMessage = 0,
                CreatedDate = DateTime.UtcNow.AddHours(7),
                PresentationId = presentationId,
            };
            context.Chat.Add(newChat);
            return context.SaveChanges();
        }

        public List<object> GetChatFromPresentation(Guid presentationId)
        {
            if (!IsPresentHasChat(presentationId))
            {
                createNewChat(presentationId);
            }
            Chat chat = context.Chat.Where(c => c.PresentationId == presentationId)
                                    .Select(c => c)
                                    .FirstOrDefault();
            List<ChatMessage> chatMessages = context.ChatMessage.Where(m => m.ChatId == chat.ChatId)
                                                                .Select(m => m).OrderBy(p => p.TimeSend)
                                                                .ToList();
            List<object> messages = new List<object>();
            foreach(var message in chatMessages)
            {
                messages.Add(new
                {
                    Sender = message.SenderName,
                    Message = message.MsgContent,
                    TimeSent = message.TimeSend,
                });
            }

            return messages;
        }

        public bool IsPresentHasChat(Guid presentationId)
        {
            return context.Chat.Any(c => c.PresentationId == presentationId);
        }

        public int AddMessage(Guid presentationId, string message, string senderEmail)
        {
            if (!IsPresentHasChat(presentationId))
            {
                createNewChat(presentationId);
            }
            Chat chat = context.Chat.Where(c => c.PresentationId == presentationId)
                                    .Select(c => c)
                                    .FirstOrDefault();
            chat.NumOfMessage = chat.NumOfMessage + 1;

            if(senderEmail == String.Empty || senderEmail == null)
            {
                ChatMessage anonyMessage = new ChatMessage
                {
                    ChatId = chat.ChatId,
                    MsgContent = message,
                    TimeSend = DateTime.UtcNow.AddHours(7),
                    SenderName = "anonymous",
                    SenderId = Guid.Empty,
                };
                context.ChatMessage.Add(anonyMessage);
                return context.SaveChanges();
            }

            Guid senderId = context.User.Where(u => u.Email == senderEmail)
                                      .Select(u => u.UserId).FirstOrDefault();
            //return -1 if email does not existed
            if(senderId == Guid.Empty)
            {
                return -1;
            }
            string username = context.User.Find(senderId).UserName;
            ChatMessage message1 = new ChatMessage
            {
                ChatId = chat.ChatId,
                MsgContent = message,
                TimeSend = DateTime.UtcNow.AddHours(7),
                SenderName = username,
                SenderId = senderId,
            };
            context.ChatMessage.Add(message1);
            return context.SaveChanges();
        }
    }
}
