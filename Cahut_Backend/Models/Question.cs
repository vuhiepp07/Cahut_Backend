namespace Cahut_Backend.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public int SlideId { get; set; }
        public string QuestionType { get; set; }
        public string RightAnswer { get; set; }
        public string Content { get; set; }

        public Slide Slide { get; set; }
        public List<Answer>Answers {get; set;}
    }
}
