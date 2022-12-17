namespace Cahut_Backend.Models
{
    public class PresentationDetail
    {
        public Guid PresentationId { get; set; }
        public Guid ColaboratorId { get; set; }
        public Presentation Presentation { get; set; }
        //public User User { get; set; }
    }
}
