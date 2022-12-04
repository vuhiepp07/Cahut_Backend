namespace Cahut_Backend.Models
{
    public class Answer
    {
        public string QuestionId { get; set; }
        public string AnswerId { get; set; }
        public string Content { get; set; }
        public int NumSelected { get; set; } 
        public Question Question { get; set; }
    }
}
