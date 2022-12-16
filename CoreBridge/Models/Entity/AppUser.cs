using CoreBridge.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CoreBridge.Models.Entity
{
    public class AppUser : CoreBridgeEntity, IAggregateRoot
    {
        public AppUser() { }

        public AppUser(string userId, string? titleCode, string password)
        {
            UserId = userId;
            TitleCode = titleCode;
            Password = password;
        }

        [Required]
        public string UserId { get; set; }

        public string? TitleCode { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
