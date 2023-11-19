namespace BugTrackerAPI.Models
{
    public class UserUpdate
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        public string? Role { get; set; }
        public string? Avatar { get; set; }

        public string? Name { get; set; }
        public override string ToString()
        {
         return $"{Name} {Role} {Avatar} {Email}";
        }
    }
}
