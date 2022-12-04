using Cahut_Backend.Repository;

namespace Cahut_Backend.Models
{
    public class SiteProvider : BaseProvider
    {
        UserRepository user;
        public UserRepository User
        {
            get
            {
                if (user == null)
                {
                    user = new UserRepository(Context);
                }
                return user;
            }
        }

        MailSenderRepository email;
        public MailSenderRepository Email
        {
            get
            {
                if (email == null)
                {
                    email = new MailSenderRepository(Context);
                }
                return email;
            }
        }

        TokenRepository token;
        public TokenRepository Token
        {
            get
            {
                if (token == null)
                {
                    token = new TokenRepository(Context);
                }
                return token;
            }
        }

        GroupRepository group;
        public GroupRepository Group
        {
            get
            {
                if (group == null)
                {
                    group = new GroupRepository(Context);
                }
                return group;
            }
        }

        PresentationRepository presentation;
        public PresentationRepository Presentation
        {
            get
            {
                if (presentation == null)
                {
                    presentation = new PresentationRepository(Context);
                }
                return presentation;
            }
        }

        SlideRepository slide;
        public SlideRepository Slide
        {
            get
            {
                if (slide == null)
                {
                    slide = new SlideRepository(Context);
                }
                return slide;
            }
        }

        QuestionRepository question;
        public QuestionRepository Question
        {
            get
            {
                if (question == null)
                {
                    question = new QuestionRepository(Context);
                }
                return question;
            }
        }

        AnswerRepository answer;
        public AnswerRepository Answer
        {
            get
            {
                if (answer == null)
                {
                    answer = new AnswerRepository(Context);
                }
                return answer;
            }
        }
    }
}
