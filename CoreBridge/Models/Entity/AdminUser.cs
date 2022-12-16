using CoreBridge.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CoreBridge.Models.Entity
{
    public class AdminUser : CoreBridgeEntity, IAggregateRoot
    {
        private AdminUser() { }

        public AdminUser(string name, string eMail, string password)
        {
            Name = name;
            EMail = eMail;
            Password = password;
        }

        public string Name { get; set; }

        [EmailAddress]
        public string EMail { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
