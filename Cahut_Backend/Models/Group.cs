namespace Cahut_Backend.Models
{
    public class Group
    {
        public Guid GroupId { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime DateCreated { get; set; }
        public int NumOfMems { get; set; }
        public string JoinGrString { get; set; }
        public bool HasPresentationPresenting { get; set; }
        public string GroupName { get; set; }
        public List<GroupDetail> GroupDetails { get; set; }
        public User User { get; set; }
    }
}
