// Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;

namespace WeeSe.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;
        public string Role { get; set; } = String.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}