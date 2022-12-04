namespace Cahut_Backend.Models
{
    public class Question
    {
        public string QuestionId { get; set; }
        public string SlideId { get; set; }
        public string QuestionType { get; set; }
        public string RightAnswer { get; set; }
        public string Content { get; set; }
        public Slide Slide { get; set; }
        public List<Answer>Answers {get; set;}
    }
}
