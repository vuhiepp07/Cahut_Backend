using Cahut_Backend.Models;

namespace Cahut_Backend.Repository
{
    public class UserSubmitChoiceRepository : BaseRepository
    {
        public UserSubmitChoiceRepository(AppDbContext context) : base(context)
        {
        }

        public bool isUserSubmitted(Guid userId, string questionId)
        {
            bool isUserSubmitted = context.UserSubmitChoice.Any(c => c.UserId == userId && c.QuestionId == questionId);
            return isUserSubmitted;
        }

        public List<object> GetChoiceResult(string questionId)
        {
            List<object> result = new List<object>();
            List<UserSubmitChoice> userSubmitChoices = context.UserSubmitChoice.Where(c => c.QuestionId == questionId).ToList();
            foreach(var userSubmitChoice in userSubmitChoices)
            {
                result.Add(new
                {
                    userName = context.User.Find(userSubmitChoice.UserId).UserName,
                    email = context.User.Find(userSubmitChoice.UserId).Email,
                    choiceContent = context.MultipleChoiceOption.Find(userSubmitChoice.OptionId).OptionContent,
                    submitTime = userSubmitChoice.SubmitTime
                });
            }
            return result;
        }
    }
}
