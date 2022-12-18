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

        MultipleChoiceSlideRepository multipleChoiceSlide;
        public MultipleChoiceSlideRepository MultipleChoiceSlide
        {
            get
            {
                if (multipleChoiceSlide == null)
                {
                    multipleChoiceSlide = new MultipleChoiceSlideRepository(Context);
                }
                return multipleChoiceSlide;
            }
        }

        MultipleChoiceQuestionRepository multipleChoiceQuestion;
        public MultipleChoiceQuestionRepository MultipleChoiceQuestion
        {
            get
            {
                if (multipleChoiceQuestion == null)
                {
                    multipleChoiceQuestion = new MultipleChoiceQuestionRepository(Context);
                }
                return multipleChoiceQuestion;
            }
        }

        MultipleChoiceOptionRepository multipleChoiceOption;
        public MultipleChoiceOptionRepository MultipleChoiceOption
        {
            get
            {
                if (multipleChoiceOption == null)
                {
                    multipleChoiceOption = new MultipleChoiceOptionRepository(Context);
                }
                return multipleChoiceOption;
            }
        }
    }
}