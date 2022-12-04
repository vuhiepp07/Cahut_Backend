namespace Cahut_Backend.Models
{
    public class Answer
    {
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public string Content { get; set; }
        public int NumSelected { get; set; } 
        public Question Question { get; set; }
    }
}
