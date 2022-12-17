namespace Cahut_Backend.Models
{
    public class MultipleChoiceQuestion : Question
    {
        public Guid MultipleChoiceQuestionId { get; set; }
        public string SlideId { get; set; }
        public string RightAnswer { get; set; }

        public MultipleChoiceSlide MultipleChoiceSlide { get; set; }
        public List<MultipleChoiceOption> MultipleChoiceOptions { get; set; }
    }
}
