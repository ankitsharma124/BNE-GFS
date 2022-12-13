using MessagePack;

namespace CoreBridge.Models.DTO
{
    [MessagePackObject]
    public class AppUserDto
    {
        public AppUserDto() { }

        public AppUserDto(string userId, string titleCode, string password)
        {
            UserId = userId;
            TitleCode = titleCode;
            Password = password;
        }

        [Key(0)]
        public String UserId { get; set; }
        [Key(1)]
        public String? TitleCode { get; set; }
        [Key(2)]
        public String Password { get; set; }
    }
}
