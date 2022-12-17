namespace Cahut_Backend.Models
{
    public class HeadingSlide : Slide
    {
        public string HeadingContent { get; set; }
        public string SubHeadingContent { get; set; }
        public Presentation Presentation { get; set; }

    }
}
