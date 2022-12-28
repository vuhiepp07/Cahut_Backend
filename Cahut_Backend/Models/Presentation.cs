namespace Cahut_Backend.Models
{
    public class Presentation
    {
        public Guid OwnerId { get; set; }
        public Guid PresentationId { get; set; }
        public string PresentationName { get; set; }
        public string PresentationType { get; set; }
        public bool IsBeingPresented { get; set; }
        public DateTime CreatedDate { get; set; }
        public User User { get; set; }
        public List<PresentationDetail> PresentationDetails { get; set; }
        public List<PresentationQuestion> PresentationQuestions { get; set; }
        public List<MultipleChoiceSlide> MultipleChoiceSlides { get; set; }
        public List<ParagraphSlide> ParagraphSlides { get; set; }
        public List<HeadingSlide> HeadingSlides { get; set; }
        public List<UserUpvoteQuestion> UserUpvoteQuestions { get; set; }
        public Chat Chat { get; set; }

    }
}
