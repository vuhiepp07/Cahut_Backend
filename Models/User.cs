namespace Cahut_Backend.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Avatar { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int AccountStatus { get; set; }

        public GroupDetail GroupDetail { get; set; }
        public Token Token { get; set; }
    }
}
