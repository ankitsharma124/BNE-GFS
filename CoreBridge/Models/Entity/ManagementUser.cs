using SendGrid.Helpers.Mail;

namespace CoreBridge.Models.Entity
{
    //COSMOS管理ユーザーテーブル
    public class ManagementUser
    {
        public ManagementUser(string userId, ManagementRoleEnum userRole, string titleCode, string email, string password)
        {
            UserId = userId;
            UserRole = userRole;
            TitleCode = titleCode;
            Email = email;
            Password = password;
        }

        public string UserId { get; set; }
        public ManagementRoleEnum UserRole { get; set; }
        public string TitleCode { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
