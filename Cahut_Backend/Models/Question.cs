namespace Cahut_Backend.Models
{
    public abstract class Question
    {
        public string QuestionId { get; set; }
        public string QuestionType { get; set; }
        public string Content { get; set; }
    }
}
