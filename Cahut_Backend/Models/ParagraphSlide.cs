namespace Cahut_Backend.Models
{
    public class ParagraphSlide : Slide
    {
        public string ParagraphContent { get; set; }
        public string HeadingContent { get; set; }
        public Presentation Presentation { get; set; }
    }
}
