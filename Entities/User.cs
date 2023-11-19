using System.ComponentModel.DataAnnotations;

namespace BugTrackerAPI.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        public string? Name { get; set; }

        [Required]
        [EmailAddress] 
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        public string? Role { get; set; }

        public string? Avatar { get; set; }
        public override string ToString()
        {
            return $"Email: {Email}, Password: {Password}";
        }
    }
}
