namespace Cahut_Backend.Models
{
    public class UserUpvoteQuestion
    {
        public string QuestionId { get; set; }
        public Guid UserId { get; set; }
        public Guid PresentationId { get; set; }
        public Guid GroupId { get; set; }
        public DateTime TimeUpVote { get; set; }

        public User User { get; set; }
        public Group Group { get; set; }
        public Presentation Presentation { get; set; }
        public PresentationQuestion PresentationQuestion { get; set; }
    }
}
