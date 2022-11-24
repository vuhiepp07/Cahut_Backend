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
    }
}
