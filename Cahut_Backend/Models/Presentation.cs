namespace Cahut_Backend.Models
{
    public class Presentation
    {
        public Guid TeacherId { get; set; }
        public Guid PresentationId { get; set; }
        public string PresentationName { get; set; }
        public DateTime CreatedDate { get; set; }
        public User User { get; set; }
        public List<Slide> Slides { get; set; }
    }
}
