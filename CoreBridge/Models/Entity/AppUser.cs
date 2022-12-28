using CoreBridge.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CoreBridge.Models.Entity
{
    public class AppUser : CoreBridgeEntity, IAggregateRoot
    {
        public AppUser() { }

        public AppUser(string userId, string? titleCode, AdminUserRoleEnum role, string? email, string password, string? updateUser, bool isDelete = false)
        {
            UserId = userId;
            TitleCode = titleCode;
            Role = role;
            Email = email;
            Password = password;
            UpdateUser = updateUser;
            IsDelete = isDelete;
        }

        public string UserId { get; set; }
        public string? TitleCode { get; set; }
        public AdminUserRoleEnum Role { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; }
        public string? UpdateUser { get; set; }
        public bool IsDelete { get; set; }
    }
}
