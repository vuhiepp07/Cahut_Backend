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

        ChatRepository chat;
        public ChatRepository Chat
        {
            get
            {
                if (chat == null)
                {
                    chat = new ChatRepository(Context);
                }
                return chat;
            }
        }

        PresentationQuestionRepository presentationQuestion;
        public PresentationQuestionRepository PresentationQuestion
        {
            get
            {
                if (presentationQuestion == null)
                {
                    presentationQuestion = new PresentationQuestionRepository(Context);
                }
                return presentationQuestion;
            }
        }

        ParagraphSlideRepository paragraphSlide;
        public ParagraphSlideRepository ParagraphSlide
        {
            get
            {
                if (paragraphSlide == null)
                {
                    paragraphSlide = new ParagraphSlideRepository(Context);
                }
                return paragraphSlide;
            }
        }

        HeadingSlideRepository headingSlide;
        public HeadingSlideRepository HeadingSlide
        {
            get
            {
                if (headingSlide == null)
                {
                    headingSlide = new HeadingSlideRepository(Context);
                }
                return headingSlide;
            }
        }

        UserUpvoteQuestionRepository userUpvoteQuestion;
        public UserUpvoteQuestionRepository UserUpvoteQuestion
        {
            get
            {
                if (userUpvoteQuestion == null)
                {
                    userUpvoteQuestion = new UserUpvoteQuestionRepository(Context);
                }
                return userUpvoteQuestion;
            }
        }

        UserSubmitChoiceRepository userSubmitChoice;
        public UserSubmitChoiceRepository UserSubmitChoice
        {
            get
            {
                if (userSubmitChoice == null)
                {
                    userSubmitChoice = new UserSubmitChoiceRepository(Context);
                }
                return userSubmitChoice;
            }
        }
    }
}