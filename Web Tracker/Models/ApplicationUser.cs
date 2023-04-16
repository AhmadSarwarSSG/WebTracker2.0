using Microsoft.AspNetCore.Identity;

namespace WebTracker.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string WebsiteUrl { get; set; }
    }
}
