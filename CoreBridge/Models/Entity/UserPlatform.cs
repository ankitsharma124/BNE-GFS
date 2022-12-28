using CoreBridge.Models.Interfaces;

namespace CoreBridge.Models.Entity
{
    public class UserPlatform : CoreBridgeEntity, IAggregateRoot
    {
        public string UserId { get; set; }
        public int PlatformType { get; set; }
        public string PlatformUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// ip_iso_code
        /// </summary>
        public string CountryCode { get; set; }

    }
}
