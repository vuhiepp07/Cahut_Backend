namespace Cahut_Backend.Models
{
    public class ResponseMessage
    {
        public Boolean status { get; set; }
        public dynamic data { get; set; }
        public string message { get; set; }
    }
}
