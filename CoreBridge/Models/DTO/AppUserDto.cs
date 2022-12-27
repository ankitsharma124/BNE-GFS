using MessagePack;

namespace CoreBridge.Models.DTO
{
    [MessagePackObject]
    public class AppUserDto
    {
        public AppUserDto() { }

        public AppUserDto(string userId, string? titleCode, AdminUserRoleEnum role, string? password, string? eMail, string? confirempassword, string? updateUser, bool isDelete = false)
        {
            UserId = userId;
            TitleCode = titleCode;
            Role = role;
            Email = eMail;
            Password = password;
            ConfirmPassword = confirempassword;
            UpdateUser = updateUser;
            IsDelete = isDelete;
        }

        [Key(0)]
        public String UserId { get; set; }
        [Key(1)]
        public String? TitleCode { get; set; }
        [Key(2)]
        public AdminUserRoleEnum Role { get; set; }
        [Key(3)]
        public String? Email { get; set; }
        [Key(4)]
        public String? Password { get; set; }
        [Key(5)]
        public String? ConfirmPassword { get; set; }
        [Key(6)]
        public String? UpdateUser { get; set; }
        [Key(7)]
        public bool IsDelete { get; set; }

        public bool IsValid()
        {
            return true;
        }

    }
}
