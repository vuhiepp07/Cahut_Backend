namespace Cahut_Backend.Models
{
    public class UserSubmitChoice
    {
        public Guid GroupId { get; set; }
        public string QuestionId { get; set; }
        public string OptionId { get; set; }
        public Guid UserId { get; set; }
        public DateTime SubmitTime {get; set;}

        public Group Group { get; set; }
        public MultipleChoiceQuestion MultipleChoiceQuestion { get; set; }
        public MultipleChoiceOption MultipleChoiceOption { get; set; }
        public User User { get; set; }
    }
}
