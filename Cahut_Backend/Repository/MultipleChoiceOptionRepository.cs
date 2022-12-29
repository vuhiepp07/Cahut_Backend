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

        public string GetMultipleChoiceQuestion(string optionId)
        {
            return context.MultipleChoiceOption.Find(optionId).QuestionId;
        }

        public int IncreaseByOne(string optionId, Guid userId)
        {
            MultipleChoiceOption option = context.MultipleChoiceOption.Find(optionId);
            option.NumSelected = option.NumSelected + 1;

            MultipleChoiceQuestion question = context.MultipleChoiceQuestion.Find(option.QuestionId);
            MultipleChoiceSlide slide = context.MultipleChoiceSlide.Find(question.SlideId);
            Presentation presentation = context.Presentation.Find(slide.PresentationId);

            if(presentation.IsBeingPresented && presentation.PresentationType == "group")
            {
                Guid groupId = context.Group.Where(g => g.PresentationId == presentation.PresentationId.ToString())
                                        .Select(g => g.GroupId).FirstOrDefault();
                UserSubmitChoice submitGroupChoice = new UserSubmitChoice
                {
                    UserId = userId,
                    QuestionId = option.QuestionId,
                    GroupId = groupId,
                    OptionId = optionId,
                    SubmitTime = DateTime.UtcNow.AddHours(7),
                };
                context.UserSubmitChoice.Add(submitGroupChoice);
            }
            if (context.SaveChanges() > 1)
            {
                return option.NumSelected;
            }
            return 0;
        }
    }
}
