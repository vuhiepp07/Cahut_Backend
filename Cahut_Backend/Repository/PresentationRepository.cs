using Cahut_Backend.Models;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Cahut_Backend.Repository
{
    public class PresentationRepository : BaseRepository
    {
        public PresentationRepository(AppDbContext context) : base(context)
        {
        }

        public bool CheckExisted(Guid userId, string presentationName)
        {
            return context.Presentation.Any(p => p.OwnerId == userId && p.PresentationName == presentationName);
        }

        public int Create(Guid userId, string presentationName)
        {
            Presentation present = new Presentation
            {
                PresentationName = presentationName,
                CreatedDate = DateTime.UtcNow.AddHours(7),
                OwnerId = userId
            };
            context.Presentation.Add(present);
            return context.SaveChanges();
        }
        public Presentation GetPresentationByNameAndTeacherId(string name, Guid teacherId)
        {
            return context.Presentation.Where(p => p.PresentationName == name && p.OwnerId == teacherId).SingleOrDefault();
        }

        public int Delete(Guid presentationId)
        {
            Presentation present = context.Presentation.Find(presentationId);
            context.Remove(present);
            return context.SaveChanges();
        }

        public int Update(Guid presentationId, Guid userId, string newName)
        {
            Presentation present = context.Presentation.Where(p => p.PresentationId == presentationId && p.OwnerId == userId).SingleOrDefault();
            present.PresentationName = newName;
            return context.SaveChanges();
        }

        public int CountNumOfSlide(Guid presentationId)
        {
            return context.Slide.Count(p => p.PresentationId == presentationId);
        }

        public List<object> GetPresentationList(Guid userId)
        {
            var res = from present in context.Presentation
                      where present.OwnerId == userId
                      select new
                      {
                          presentationId = present.PresentationId,
                          createdDate = present.CreatedDate,
                          presentationName = present.PresentationName,
                      };
            List<object> result = new List<object>();
            foreach(var item in res)
            {
                int NumOfSlides = 0;
                NumOfSlides += context.MultipleChoiceSlide.Count(p => p.PresentationId == item.presentationId);
                NumOfSlides += context.ParagraphSlide.Count(p => p.PresentationId == item.presentationId);
                NumOfSlides += context.HeadingSlide.Count(p => p.PresentationId == item.presentationId);

                result.Add(new
                {
                    presentationId = item.presentationId,
                    createdDate = item.createdDate,
                    presentationName = item.presentationName,
                    numOfSlides = NumOfSlides
                });
            }
            return result;
        }
        public bool presentationExisted(Guid presentationId, Guid userId)
        {
            return context.Presentation.Any(p => p.PresentationId == presentationId && p.OwnerId == userId);
        }

        public string GetPresentationName(Guid presentationId)
        {
            return context.Presentation.Find(presentationId).PresentationName;
        }

        public List<object> GetPresentationSlides(Guid presentationId)
        {
            List<MultipleChoiceSlide> multipleChoiceSlides = context.MultipleChoiceSlide.Where(p => p.PresentationId == presentationId).ToList<MultipleChoiceSlide>();
            List<ParagraphSlide> paragraphSlides = context.ParagraphSlide.Where(p => p.PresentationId == presentationId).ToList<ParagraphSlide>();
            List<HeadingSlide> headingSLides = context.HeadingSlide.Where(p => p.PresentationId == presentationId).ToList<HeadingSlide>();
            List<Slide> totalSlides = new List<Slide>();
            totalSlides.AddRange(multipleChoiceSlides);
            totalSlides.AddRange(paragraphSlides);
            totalSlides.AddRange(headingSLides);
            var totalSlidesSorted = totalSlides.OrderBy(p => p.DateCreated);
            List<object> result = new List<object> { totalSlidesSorted };
            return result;
        }

        public int CountPresentationOwned(Guid userId)
        {
            return context.Presentation.Count(p => p.OwnerId == userId);
        }
    }
}
