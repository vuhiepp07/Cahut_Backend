using Cahut_Backend.Models;

namespace Cahut_Backend.Repository
{
    public class AnswerRepository : BaseRepository
    {
        public AnswerRepository(AppDbContext context) : base(context)
        {
        }

        public bool AnswerIdExisted(string answerId)
        {
            return context.Answer.Any(p => p.AnswerId == answerId);
        }

        public int Add(string answerId, string questionId,string content)
        {
            Answer ans = new Answer
            {
                AnswerId = answerId,
                QuestionId = questionId,
                Content = content
            };
            context.Answer.Add(ans);
            return context.SaveChanges();
        }

        public int Update(string answerId,string content)
        {
            Answer ans = context.Answer.Find(answerId);
            ans.Content = content;
            return context.SaveChanges();
        }

        public int Delete(string answerId)
        {
            Answer ans = context.Answer.Find(answerId);
            context.Answer.Remove(ans);
            return context.SaveChanges();
        }

        public List<object> GetQuestionAnswer(string questionId)
        {
            var res = (from ans in context.Answer
                      where ans.QuestionId == questionId
                      select new
                      {
                          content = ans.Content,
                          answerId = ans.AnswerId,
                          numSelected = ans.NumSelected,
                          QuestionId = ans.QuestionId
                      }).ToList<object>();
            return res;
        }
    }
}
