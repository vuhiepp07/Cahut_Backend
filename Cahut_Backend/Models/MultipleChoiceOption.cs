namespace Cahut_Backend.Models
{
    public class MultipleChoiceOption
    {
        public string QuestionId { get; set; }
        public string OptionId { get; set; }
        public string OptionContent { get; set; }
        public int NumSelected { get; set; }
        public DateTime CreatedDate { get; set; }

        public MultipleChoiceQuestion MultipleChoiceQuestion { get; set; }
        public List<UserSubmitChoice> UserSubmitChoices { get; set; }

    }
}
