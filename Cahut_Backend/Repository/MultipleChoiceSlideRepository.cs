using Cahut_Backend.Models;

namespace Cahut_Backend.Repository
{
    public class MultipleChoiceSlideRepository : BaseRepository
    {
        public MultipleChoiceSlideRepository(AppDbContext context) : base(context)
        {
        }

        public int CreateMultipleChoiceSlide(Guid PresentationId, string slideId)
        {
            MultipleChoiceSlide slide = new MultipleChoiceSlide
            {
                PresentationId = PresentationId,
                SlideId = slideId,
                IsCurrent = 0,
                SlideType = "MultipleChoice",
                DateCreated = DateTime.UtcNow.AddHours(7),
            };
            context.MultipleChoiceSlide.Add(slide);
            return context.SaveChanges();
        }

        public bool CheckSlideIdExisted(string id)
        {
            return context.MultipleChoiceSlide.Any(p => p.SlideId == id);
        }

        public int Delete(string slideId)
        {
            MultipleChoiceSlide slide = context.MultipleChoiceSlide.Find(slideId);
            context.MultipleChoiceSlide.Remove(slide);
            return context.SaveChanges();
        }

        public Guid GetPresentationId(string slideId)
        {
            Guid presentationId = context.MultipleChoiceSlide.Find(slideId).PresentationId;
            return presentationId;
        }

        public List<object> GetMultipleChoiceQuestion(Guid presentationId)
        {
            var questions = from slide in context.MultipleChoiceSlide
                                    join multipleQuestion in context.MultipleChoiceQuestion
                                    on slide.SlideId equals multipleQuestion.SlideId
                                    where slide.PresentationId == presentationId
                                    select multipleQuestion;
            List<object> multipleChoiceQuestion = new List<object>();
            foreach(var question in questions)
            {
                multipleChoiceQuestion.Add(new {content = question.Content, questionId = question.QuestionId});
            }
            return multipleChoiceQuestion;
        }
    }
}
