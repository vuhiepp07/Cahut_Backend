using Cahut_Backend.Models;

namespace Cahut_Backend.Repository
{
    public class ParagraphSlideRepository : BaseRepository
    {
        public ParagraphSlideRepository(AppDbContext context) : base(context)
        {
        }
        public int CreateParagraphSlide(Guid PresentationId, string slideId)
        {
            ParagraphSlide slide = new ParagraphSlide
            {
                PresentationId = PresentationId,
                SlideId = slideId,
                DateCreated = DateTime.UtcNow.AddHours(7),
                SlideType = "Paragraph",
                IsCurrent = 0,
                HeadingContent = "",
                ParagraphContent = ""
            };
            context.ParagraphSlide.Add(slide);
            return context.SaveChanges();
        }

        public int DeleteParagraphSlide(string slideId)
        {
            ParagraphSlide paragraphSlide = context.ParagraphSlide.Find(slideId);
            context.ParagraphSlide.Remove(paragraphSlide);
            return context.SaveChanges();
        }

        public int UpdateParagraphSlide(string slideId, string headingContent, string paragraphContent)
        {
            ParagraphSlide paragraphSlide = context.ParagraphSlide.Find(slideId);
            paragraphSlide.HeadingContent = headingContent;
            paragraphSlide.ParagraphContent = paragraphContent;
            return context.SaveChanges();
        }

        public object GetParagraphSlide(string slideId)
        {
            var slide = context.ParagraphSlide.Find(slideId);
            return new
            {
                headingContent = slide.HeadingContent,
                paragraphContent = slide.ParagraphContent
            };
        }

        public bool CheckSlideIdExisted(string id)
        {
            return context.ParagraphSlide.Any(p => p.SlideId == id);
        }

        public Guid GetPresentationId(string slideId)
        {
            return context.ParagraphSlide.Find(slideId).PresentationId;
        }
    }
}
