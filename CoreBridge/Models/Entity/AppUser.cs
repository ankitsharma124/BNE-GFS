using CoreBridge.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CoreBridge.Models.Entity
{
    public class AppUser : CoreBridgeEntity, IAggregateRoot
    {
        [Required]
        public string UserId { get; set; }

        public string? TitleCode { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
