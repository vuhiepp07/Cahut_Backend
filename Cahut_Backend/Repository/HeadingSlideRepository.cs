using Cahut_Backend.Models;

namespace Cahut_Backend.Repository
{
    public class HeadingSlideRepository : BaseRepository
    {
        public HeadingSlideRepository(AppDbContext context) : base(context)
        {
        }

        public int CreateHeadingSlide(Guid PresentationId, string slideId)
        {
            HeadingSlide slide = new HeadingSlide
            {
                PresentationId = PresentationId,
                SlideId = slideId,
                DateCreated = DateTime.UtcNow.AddHours(7),
                SlideType = "Heading",
                IsCurrent = 0,
                HeadingContent = "",
                SubHeadingContent = "",
            };
            context.HeadingSlide.Add(slide);
            return context.SaveChanges();
        }

        public int EditHeadingSlide(string slideId, string headingContent, string subHeadingContent)
        {
            HeadingSlide slide = context.HeadingSlide.Find(slideId);
            slide.HeadingContent = headingContent;
            slide.SubHeadingContent = subHeadingContent;
            return context.SaveChanges();
        }

        public int DeleteHeadingSlide(string slideId)
        {
            HeadingSlide slide = context.HeadingSlide.Find(slideId);
            context.HeadingSlide.Remove(slide);
            return context.SaveChanges();
        }

        public object GetHeadingSlide(string slideId)
        {
            var slide = context.HeadingSlide.Find(slideId);
            return new
            {
                headingContent = slide.HeadingContent,
                subHeadingContent = slide.SubHeadingContent
            };
        }

        public bool CheckSlideIdExisted(string id)
        {
            return context.HeadingSlide.Any(p => p.SlideId == id);
        }
       
        public Guid GetPresentationId(string slideId)
        {
            return context.HeadingSlide.Find(slideId).PresentationId;
        }
    }
}
