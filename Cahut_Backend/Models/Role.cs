namespace Cahut_Backend.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public List<GroupDetail> GroupDetails { get; set; }
    }
}
