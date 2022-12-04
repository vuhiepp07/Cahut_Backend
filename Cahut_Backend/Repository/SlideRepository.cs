using Cahut_Backend.Models;

namespace Cahut_Backend.Repository
{
    public class SlideRepository : BaseRepository
    {
        public SlideRepository(AppDbContext context) : base(context)
        {
        }

        public int Create(Guid PresentationId, string slideId)
        {
            Slide slide = new Slide
            {
                PresentationId = PresentationId,
                SlideId = slideId,
                DateCreated = DateTime.UtcNow.AddHours(7),
            };
            context.Add(slide);
            return context.SaveChanges();
        }

        public bool CheckSlideIdExisted(string id)
        {
            return context.Slide.Any(p => p.SlideId == id);
        }

        public int Delete(string slideId)
        {
            Slide slide = context.Slide.Find(slideId);
            context.Slide.Remove(slide);
            return context.SaveChanges();
        }
    }
}
