using Cahut_Backend.Models;

namespace Cahut_Backend.Repository
{
    public class PresentationQuestionRepository : BaseRepository
    {
        public PresentationQuestionRepository(AppDbContext context) : base(context)
        {
        }

        public int SendQuestion(Guid presentationId, string questionContent)
        {
            string questionId = Helper.RandomString(8);
            if(context.PresentationQuestion.Any(q => q.QuestionId == questionId))
            {
                return 0;
            }
            PresentationQuestion presentationQuestion = new PresentationQuestion
            {
                QuestionId = questionId,
                QuestionType = "Presentation",
                Content = questionContent,
                isAnswered = false,
                NumUpVote = 0,
                PresentationId = presentationId,
            };
            context.PresentationQuestion.Add(presentationQuestion);
            return context.SaveChanges();
        }

        public int UpVoteQuestion(string questionId)
        {
            PresentationQuestion presentationQuestion = context.PresentationQuestion.Find(questionId);
            if(questionId != null)
            {
                presentationQuestion.NumUpVote = presentationQuestion.NumUpVote + 1;
                return context.SaveChanges();
            }
            return 0;
        }

        public int UnUpVoteQuestion(string questionId)
        {
            PresentationQuestion presentationQuestion = context.PresentationQuestion.Find(questionId);
            if (questionId != null)
            {
                presentationQuestion.NumUpVote = presentationQuestion.NumUpVote - 1;
                return context.SaveChanges();
            }
            return 0;
        }

        public List<Object> GetPresentationQuestions(Guid presentationId)
        {
            List<Object> presentationQuestions = new List<Object>();
            List<PresentationQuestion> questionList = context.PresentationQuestion.Where(q => q.PresentationId == presentationId)
                                                                                    .Select(q => q).ToList();
            foreach(var question in questionList)
            {
                presentationQuestions.Add(new
                {
                    questionId = question.QuestionId,
                    question = question.Content,
                    numUpVote = question.NumUpVote,
                    isAnswered = question.isAnswered
                });
            }
            return presentationQuestions;
        }

        public int UpdateQuestionAnswered(string questionId)
        {
            PresentationQuestion presentationQuestion = context.PresentationQuestion.Find(questionId);
            if(presentationQuestion != null)
            {
                presentationQuestion.isAnswered = !presentationQuestion.isAnswered;
                return context.SaveChanges();
            }
            return 0;
        }

        public bool IsQuestionExisted(string questionId)
        {
            return context.PresentationQuestion.Any(q => q.QuestionId == questionId);
        }

        public Guid GetPresentationId(string questionId)
        {
            return context.PresentationQuestion.Find(questionId).PresentationId;
        }
    }
}
