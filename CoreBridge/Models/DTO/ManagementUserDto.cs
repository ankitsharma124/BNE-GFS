using MessagePack;

namespace CoreBridge.Models.DTO
{
    [MessagePackObject]
    public class ManagementUserDto
    {
        public ManagementUserDto(string userId, ManagementRoleEnum userRole, string titleCode, string email, string password, string confirmPassword)
        {
            UserId = userId;
            UserRole = userRole;
            TitleCode = titleCode;
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
        }

        [Key(0)]
        public String UserId { get; set; }
        [Key(1)]
        public ManagementRoleEnum UserRole { get; set; }
        [Key(2)]
        public String TitleCode { get; set; }
        [Key(3)]
        public String Email { get; set; }
        [Key(4)]
        public String Password { get; set; }
        [Key(5)]
        public String ConfirmPassword { get; set; }
    }
}
