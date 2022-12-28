namespace Cahut_Backend.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Avatar { get; set; }
        public string UserName { get; set; }
        public byte[] Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int AccountStatus { get; set; }
        public string RefreshToken { get; set; }
        public string ResetPasswordString { get; set; }
        public DateTime RefreshTokenExpiredTime { get; set; }
        //public List<PresentationDetail> PresentationDetails { get; set; }
        public List<Group> Group { get; set; }
        public List<Presentation> Presentations { get; set; }
        public List<UserSubmitChoice> UserSubmitChoices { get; set; }
        public List<UserUpvoteQuestion> UserUpvoteQuestions { get; set; }
    }
}
