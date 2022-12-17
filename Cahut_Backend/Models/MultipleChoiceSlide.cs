namespace Cahut_Backend.Models
{
    public class MultipleChoiceSlide : Slide
    {
        //mac dinh bang 1, neu nhu yeu cau 1 slide co nhieu question thi moi thay doi
        public int NumOfMultipleChoiceQuestion { get; set; }
        public List<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; }
        public Presentation Presentation { get; set; }

    }
}
