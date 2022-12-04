namespace Cahut_Backend.Models
{
    public class Slide
    {
        public int SlideId { get; set; }
        public int SlideOrder { get; set; }
        public Guid PresentationId { get; set; }
        public DateTime DateCreated { get; set; }
        public Presentation Presentation { get; set; }
        public Question Question { get; set; }
    }
}
