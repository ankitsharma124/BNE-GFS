using MessagePack;

namespace CoreBridge.Models.DTO
{
    [MessagePackObject]
    public class AppUserDto
    {
        public AppUserDto() { }

        public AppUserDto(string userId, string titleCode, string password, string confirempassword)
        {
            UserId = userId;
            TitleCode = titleCode;
            Password = password;
            ConfirmPassword = confirempassword;
        }

        [Key(0)]
        public String UserId { get; set; }
        [Key(1)]
        public String? TitleCode { get; set; }
        [Key(2)]
        public String Password { get; set; }
        [Key(3)]
        public String ConfirmPassword { get; set; }

        public bool IsValid()
        {
            return true;
        }

    }
}
