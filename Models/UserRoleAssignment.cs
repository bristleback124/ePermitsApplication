namespace ePermits.Models
{
    // One row per role a user holds. Users.UserRoleId stays as the primary role and is always one of these.
    public class UserRoleAssignment
    {
        public int UserId { get; set; }
        public int UserRoleId { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public UserRole? UserRole { get; set; }
    }
}
