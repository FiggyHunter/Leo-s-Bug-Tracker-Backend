namespace BugTrackerAPI.Models
{
    public class UserRegister
    {
        // Step 1
        public string Email { get; set; }
        public string Password { get; set; }

        // Step 2 
        public string? Role { get; set; }
        public string? Avatar { get; set; }

        // Step 3
        public string? Name { get; set; }
        public override string ToString()
        {
            return $"Email: {Email}, Password: {Password}";
        }
    }
}
