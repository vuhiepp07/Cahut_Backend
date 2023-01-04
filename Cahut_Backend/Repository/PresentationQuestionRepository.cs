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
            if (context.PresentationQuestion.Any(q => q.QuestionId == questionId))
            {
                return 0;
            }
            PresentationQuestion presentationQuestion = new PresentationQuestion
            {
                QuestionId = questionId,
                QuestionType = "Presentation",
                Content = questionContent,
                isAnswered = false,
                CreatedDate = DateTime.UtcNow.AddHours(7),
                NumUpVote = 0,
                PresentationId = presentationId,
                UserUpvoteQuestions = new List<UserUpvoteQuestion>(),
            };
            context.PresentationQuestion.Add(presentationQuestion);
            return context.SaveChanges();
        }

        public bool IsVote(string questionId, Guid userId)
        {
            return context.PresentationQuestion.Any(q => q.QuestionId == questionId && q.UserUpvoteQuestions.Any(q => q.UserId == userId && q.QuestionId == questionId));
        }

        public int UpVoteQuestion(string questionId, Guid userId)
        {
            PresentationQuestion presentationQuestion = context.PresentationQuestion.Find(questionId);
            if (questionId != null)
            {
                Guid presentationId = GetPresentationId(questionId);
                Group presetatingGroup = context.Group.Where(group => group.PresentationId == presentationId.ToString()).Select(g => g).FirstOrDefault();
                presentationQuestion.NumUpVote = presentationQuestion.NumUpVote + 1;
                if (userId != Guid.Empty && presetatingGroup != null)
                {
                    UserUpvoteQuestion upvoteHistory = new UserUpvoteQuestion
                    {
                        QuestionId = questionId,
                        PresentationId = presentationQuestion.PresentationId,
                        GroupId = presetatingGroup != null ? presetatingGroup.GroupId : Guid.Empty,
                        UserId = userId,
                        TimeUpVote = DateTime.UtcNow.AddHours(7),
                    };
                    context.UserUpvoteQuestion.Add(upvoteHistory);

                }
                return context.SaveChanges();
            }
            return 0;
        }

        public int UnUpVoteQuestion(string questionId, Guid userId)
        {
            PresentationQuestion presentationQuestion = context.PresentationQuestion.Find(questionId);
            if (questionId != null)
            {
                Guid presentationId = GetPresentationId(questionId);
                Group presetatingGroup = context.Group.Where(group => group.PresentationId == presentationId.ToString()).Select(g => g).FirstOrDefault();
                presentationQuestion.NumUpVote = presentationQuestion.NumUpVote - 1;

                UserUpvoteQuestion userUpvoteQuestion = context.UserUpvoteQuestion.Where(q => q.QuestionId == questionId && q.UserId == userId).FirstOrDefault();
                if (userUpvoteQuestion != null)
                {
                    context.UserUpvoteQuestion.Remove(userUpvoteQuestion);
                }
                //if (userId != Guid.Empty)
                //{


                //    UserUpvoteQuestion upvoteHistory = new UserUpvoteQuestion
                //    {
                //        QuestionId = questionId,
                //        PresentationId = presentationQuestion.PresentationId,
                //        GroupId = presetatingGroup != null ? presetatingGroup.GroupId : Guid.Empty,
                //        UserId = userId,
                //        TimeUpVote = DateTime.UtcNow.AddHours(7),
                //    };
                //    context.UserUpvoteQuestion.Add(upvoteHistory);

                //}
                return context.SaveChanges();
            }
            return 0;
        }

        public List<Object> GetPresentationQuestions(Guid presentationId, Guid userId)
        {

            List<Object> presentationQuestions = new List<Object>();
            List<PresentationQuestion> questionList = context.PresentationQuestion.Where(q => q.PresentationId == presentationId)
                                                                                    .Select(q => q).OrderBy(p => p.CreatedDate).ToList();
            foreach (var question in questionList)
            {
                presentationQuestions.Add(new
                {
                    isUpvote = context.UserUpvoteQuestion.Any(q => q.UserId == userId && q.QuestionId == question.QuestionId),
                    questionId = question.QuestionId,
                    question = question.Content,
                    numUpVote = question.NumUpVote,
                    isAnswered = question.isAnswered
                }); ; ;
            }
            return presentationQuestions;
        }

        public List<object> GetAnsweredQuestions(Guid presentationId, Guid userId)
        {
            List<Object> presentationQuestions = new List<Object>();
            List<PresentationQuestion> questionList = context.PresentationQuestion.Where(q => q.PresentationId == presentationId && q.isAnswered == true)
                                                                                    .Select(q => q).OrderBy(p => p.CreatedDate).ToList();
            foreach (var question in questionList)
            {
                presentationQuestions.Add(new
                {
                    isUpvote = context.UserUpvoteQuestion.Any(q => q.UserId == userId && q.QuestionId == question.QuestionId),
                    questionId = question.QuestionId,
                    question = question.Content,
                    numUpVote = question.NumUpVote,
                    isAnswered = question.isAnswered
                }); ; ;
            }
            return presentationQuestions;
        }

        public List<object> GetUnAnsweredQuestions(Guid presentationId, Guid userId)
        {
            List<Object> presentationQuestions = new List<Object>();
            List<PresentationQuestion> questionList = context.PresentationQuestion.Where(q => q.PresentationId == presentationId && q.isAnswered == false)
                                                                                    .Select(q => q).OrderBy(p => p.CreatedDate).ToList();
            foreach (var question in questionList)
            {
                presentationQuestions.Add(new
                {
                    isUpvote = context.UserUpvoteQuestion.Any(q => q.UserId == userId && q.QuestionId == question.QuestionId),
                    questionId = question.QuestionId,
                    question = question.Content,
                    numUpVote = question.NumUpVote,
                    isAnswered = question.isAnswered
                }); ; ;
            }
            return presentationQuestions;
        }

        public int UpdateQuestionAnswered(string questionId)
        {
            PresentationQuestion presentationQuestion = context.PresentationQuestion.Find(questionId);
            if (presentationQuestion != null)
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

        public List<object> GetQuestionsSortedByTime(Guid presentationId, Guid userId, string sortType)
        {
            List<Object> presentationQuestions = new List<Object>();
            List<PresentationQuestion> questionList = new List<PresentationQuestion>();
            if (sortType == "Descending")
            {
                questionList = context.PresentationQuestion.Where(q => q.PresentationId == presentationId)
                                                                                    .Select(q => q).OrderByDescending(p => p.CreatedDate).ToList();
            }
            if(sortType == "Ascending")
            {
                questionList = context.PresentationQuestion.Where(q => q.PresentationId == presentationId)
                                                                                    .Select(q => q).OrderBy(p => p.CreatedDate).ToList();
            }
            foreach (var question in questionList)
            {
                presentationQuestions.Add(new
                {
                    isUpvote = context.UserUpvoteQuestion.Any(q => q.UserId == userId && q.QuestionId == question.QuestionId),
                    questionId = question.QuestionId,
                    question = question.Content,
                    numUpVote = question.NumUpVote,
                    isAnswered = question.isAnswered
                }); ; ;
            }
            return presentationQuestions;
        }

        public List<object> GetQuestionsSortedByVote(Guid presentationId, Guid userId, string sortType)
        {
            List<Object> presentationQuestions = new List<Object>();
            List<PresentationQuestion> questionList = new List<PresentationQuestion>();
            if (sortType == "Descending")
            {
                questionList = context.PresentationQuestion.Where(q => q.PresentationId == presentationId)
                                                                                    .Select(q => q).OrderByDescending(p => p.NumUpVote).ToList();
            }
            if (sortType == "Ascending")
            {
                questionList = context.PresentationQuestion.Where(q => q.PresentationId == presentationId)
                                                                                    .Select(q => q).OrderBy(p => p.NumUpVote).ToList();
            }
            foreach (var question in questionList)
            {
                presentationQuestions.Add(new
                {
                    isUpvote = context.UserUpvoteQuestion.Any(q => q.UserId == userId && q.QuestionId == question.QuestionId),
                    questionId = question.QuestionId,
                    question = question.Content,
                    numUpVote = question.NumUpVote,
                    isAnswered = question.isAnswered
                }); ; ;
            }
            return presentationQuestions;
        }
    }
}
