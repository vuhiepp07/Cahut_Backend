namespace Cahut_Backend.Models
{
    public class Answer
    {
        public int AnswerId { get; set; }
        public string Content { get; set; }

        public Question Question { get; set; }
    }
}
