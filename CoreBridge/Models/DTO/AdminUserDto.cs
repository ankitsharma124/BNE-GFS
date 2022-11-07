namespace CoreBridge.Models.DTO
{
    public class AdminUserDto
    {
        public AdminUserDto() { }

        public AdminUserDto(string name, string eMail, string password, string confirm_password)
        {
            Name = name;
            EMail = eMail;
            Password = password;
            ConfirmPassword = confirm_password;
        }

        public String Name { get; set; }
        public String EMail { get; set; }
        public String Password { get; set; }
        public String ConfirmPassword { get; set; }

        public bool IsValid()
        {
            return true;
        }
    }
}
