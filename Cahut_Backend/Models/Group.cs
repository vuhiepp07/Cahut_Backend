namespace Cahut_Backend.Models
{
    public class Group
    {
        public Guid GroupId { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime DateCreated { get; set; }
        public string JoinGrLink { get; set; }

        public GroupDetail GroupDetail { get; set; }
    }
}
