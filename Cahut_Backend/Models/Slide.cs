namespace Cahut_Backend.Models
{
    public class Slide
    {
        public int SlideId { get; set; }
        public Guid PresentationId { get; set; }

        public Presentation Presentation { get; set; }
    }
}
