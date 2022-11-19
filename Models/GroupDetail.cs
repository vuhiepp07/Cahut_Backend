namespace Cahut_Backend.Models
{
    public class GroupDetail
    {
        public Guid GroupId { get; set; }
        public Guid MemberId { get; set; }
        public int RoleId { get; set; }
        public DateTime JoinedDate { get; set; }

        public Group Group { get; set; }
        public User User { get; set; }
    }
}
