using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SignalR_Check.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(20, ErrorMessage = "Username cannot be longer than 20 characters\n")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "ConfirmPassword must match with Password\n")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string WebsiteUrl { get; set; }

    }


}
