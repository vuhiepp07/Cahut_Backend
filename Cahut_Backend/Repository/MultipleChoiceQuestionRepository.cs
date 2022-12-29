using Cahut_Backend.Models;

namespace Cahut_Backend.Repository
{
    public class MultipleChoiceQuestionRepository : BaseRepository
    {
        public MultipleChoiceQuestionRepository(AppDbContext context) : base(context)
        {
        }

        public bool CheckSlideAlreadyHaveQues(string slideId)
        {
            return context.MultipleChoiceQuestion.Any(p => p.SlideId == slideId);
        }

        public int AddToSlide(string slideId, string questionId, string type, string content)
        {
            MultipleChoiceQuestion ques = new MultipleChoiceQuestion
            {
                SlideId = slideId,
                QuestionId = questionId,
                QuestionType = type,
                Content = content,
            };
            context.MultipleChoiceQuestion.Add(ques);
            return context.SaveChanges();
        }

        public bool CheckExistedId(string questionId)
        {
            return context.MultipleChoiceQuestion.Any(p => p.QuestionId == questionId);
        }

        public object GetSlideQuestion(string slideId)
        {
            var res = (from ques in context.MultipleChoiceQuestion
                       where ques.SlideId == slideId
                       select new
                       {
                           SlideId = ques.SlideId,
                           QuestionId = ques.QuestionId,
                           QuestionType = ques.QuestionType,
                           Content = ques.Content,
                       }).SingleOrDefault();
            return res;
        }

        public int Update(string questionId, string type, string content)
        {
            Question ques = context.MultipleChoiceQuestion.Find(questionId);
            ques.QuestionType = type;
            ques.Content = content;
            return context.SaveChanges();
        }

        public int Delete(string questionId)
        {
            MultipleChoiceQuestion ques = context.MultipleChoiceQuestion.Find(questionId);
            context.MultipleChoiceQuestion.Remove(ques);
            return context.SaveChanges();
        }

        public string DeleteWithSlide(string slideId)
        {
            MultipleChoiceQuestion question = context.MultipleChoiceQuestion.Where(p => p.SlideId == slideId).SingleOrDefault();
            string questionId = question.QuestionId;
            context.MultipleChoiceQuestion.Remove(question);
            context.SaveChanges();
            return questionId;
        }

        public bool IsUserSubmitted(Guid userId, string questionId)
        {
            return context.UserSubmitChoice.Any(c => c.UserId == userId && c.QuestionId == questionId);
        }
    }
}
