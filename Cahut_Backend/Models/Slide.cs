namespace Cahut_Backend.Models
{
    public abstract class Slide
    {
        public string SlideId { get; set; }
        public int SlideOrder { get; set; }
        public string SlideType { get; set; }
        public int IsCurrent { get; set; }   
        public Guid PresentationId { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
