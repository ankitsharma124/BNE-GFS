using MessagePack;

namespace CoreBridge.Models.DTO
{
    [MessagePackObject]
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

        //ここがユーザー情報になる
        //TitleCodeとかをここへ追加するのか！？

        [Key(0)]
        public String Name { get; set; }
        [Key(1)]
        public String EMail { get; set; }
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
