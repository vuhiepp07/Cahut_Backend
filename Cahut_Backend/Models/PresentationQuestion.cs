namespace Cahut_Backend.Models
{
    public class PresentationQuestion : Question
    {
        public Guid PresentationId { get; set; }
        public int NumUpVote { get; set; }
        public bool isAnswered { get; set; }
        public DateTime CreatedDate { get; set; }
        public Presentation Presentation { get; set; }
        public List<UserUpvoteQuestion> UserUpvoteQuestions { get; set; }

    }
}
