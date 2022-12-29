namespace CoreBridge.Models.DTO
{
    public class AppUserSearchDto
    {
        public AppUserSearchDto() { }

        public AppUserSearchDto(string? userId, AdminUserRoleEnum? userRole, string? titleCode, DateTime? createdDate, DateTime? updatedDate, bool resetData)
        {
            UserId = userId;
            UserRole = userRole;
            TitleCode = titleCode;
            CreatedDate = createdDate;
            UpdatedDate = updatedDate;
            ResetData = resetData;
        }

        public string? UserId { get; set; }
        public AdminUserRoleEnum? UserRole { get; set; }
        public string? TitleCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool ResetData { get; set; }
    }
}
