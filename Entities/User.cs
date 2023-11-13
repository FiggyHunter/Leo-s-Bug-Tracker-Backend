﻿using System.ComponentModel.DataAnnotations;

namespace BugTrackerAPI.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        [Required]
        [EmailAddress] 
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        public string Role { get; set; }

        public string Avatar { get; set; }
    }
}
