using CoreBridge.Models.Interfaces;

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
        public string EMail { get; set; }
        public string Password { get; set; }
    }
}
