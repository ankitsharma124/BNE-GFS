using CoreBridge.Models;
using Microsoft.AspNetCore.Authorization;

namespace CoreBridge.Attributes
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {

        public AuthorizeRolesAttribute(params AdminUserRoleEnum[] roles)
        {
            Roles = string.Join(", ", roles);
        }
    }
}
