using Cahut_Backend.Models;

namespace Cahut_Backend.Repository
{
    public class QuestionRepository : BaseRepository
    {
        public QuestionRepository(AppDbContext context) : base(context)
        {
        }

        public bool CheckSlideAlreadyHaveQues(string slideId)
        {
            return context.Question.Any(p => p.SlideId == slideId);
        }

        public int AddToSlide(string slideId, string questionId, string type, string content)
        {
            Question ques = new Question
            {
                SlideId = slideId,
                QuestionId = questionId,
                QuestionType = type,
                Content = content,
            };
            context.Question.Add(ques);
            return context.SaveChanges();
        }

        public bool CheckExistedId(string questionId)
        {
            return context.Question.Any(p => p.QuestionId == questionId);
        }

        public object GetSlideQuestion(string slideId)
        {
            var res = (from ques in context.Question
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
            Question ques = context.Question.Find(questionId);
            ques.QuestionType = type;
            ques.Content = content; 
            return context.SaveChanges();
        }

        public int Delete(string questionId)
        {
            Question ques = context.Question.Find(questionId);
            context.Question.Remove(ques);
            return context.SaveChanges();
        }

        public string DeleteWithSlide(string slideId)
        {
            Question question = context.Question.Where(p => p.SlideId == slideId).SingleOrDefault();
            string questionId = question.QuestionId;
            context.Question.Remove(question);
            context.SaveChanges();
            return questionId;
        }
    }
}
