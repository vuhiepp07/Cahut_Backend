namespace Cahut_Backend.Models
{
    public class Group
    {
        public Guid GroupId { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime DateCreated { get; set; }
        public string JoinGrString { get; set; }
        public string GroupName { get; set; }
        public GroupDetail GroupDetail { get; set; }
    }
}
