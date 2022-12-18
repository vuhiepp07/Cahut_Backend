using Cahut_Backend.Models;

namespace Cahut_Backend.Repository
{
    public class MultipleChoiceOptionRepository: BaseRepository
    {
        public MultipleChoiceOptionRepository(AppDbContext context) : base(context)
        {
        }

        public bool MultipleChoiceOptionIdExisted(string optionId)
        {
            return context.MultipleChoiceOption.Any(p => p.OptionId == optionId);
        }

        public int Add(string optionId, string questionId, string content)
        {
            MultipleChoiceOption option = new MultipleChoiceOption
            {
                OptionId = optionId,
                CreatedDate = DateTime.UtcNow.AddHours(7),
                QuestionId = questionId,
                OptionContent = content
            };
            context.MultipleChoiceOption.Add(option);
            return context.SaveChanges();
        }

        public int Update(string optionId, string content)
        {
            MultipleChoiceOption option = context.MultipleChoiceOption.Find(optionId);
            option.OptionContent = content;
            return context.SaveChanges();
        }

        public int Delete(string optionId)
        {
            MultipleChoiceOption option = context.MultipleChoiceOption.Find(optionId);
            context.MultipleChoiceOption.Remove(option);
            return context.SaveChanges();
        }

        public int DeleteOptionWithQuestion(string questionId)
        {
            List<MultipleChoiceOption> lstOptions = (from option in context.MultipleChoiceOption
                                                 where option.QuestionId == questionId
                                   select option).ToList<MultipleChoiceOption>();
            context.MultipleChoiceOption.RemoveRange(lstOptions);
            return context.SaveChanges();
        }

        public List<object> GetMultipleChoiceQuestionOptions(string questionId)
        {
            var res = (from option in context.MultipleChoiceOption
                       orderby option.CreatedDate
                       where option.QuestionId == questionId
                       select new
                       {
                           content = option.OptionContent,
                           optionId = option.OptionId,
                           numSelected = option.NumSelected,
                           QuestionId = option.QuestionId
                       }).ToList<object>();
            return res;
        }

        public int IncreaseByOne(string optionId)
        {
            MultipleChoiceOption option = context.MultipleChoiceOption.Find(optionId);
            option.NumSelected = option.NumSelected + 1;
            if (context.SaveChanges() > 1)
            {
                return option.NumSelected;
            }
            return 0;
        }
    }
}
