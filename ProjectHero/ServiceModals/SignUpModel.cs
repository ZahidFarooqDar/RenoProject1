﻿using System.ComponentModel.DataAnnotations;

namespace ProjectHero.ServiceModals
{
    public class SignUpModel
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; } // Ensure this is a string

        [Required]
        [Compare("Password")] // Compare to the Password property
        public string? ConfirmPassword { get; set; }
    }
}
