using Ardalis.Specification;
using CoreBridge.Models.Entity;

namespace CoreBridge.Specifications
{
    public class UserPlatformSpecification : Specification<UserPlatform>
    {
        public UserPlatformSpecification()
        {
        }

        public void FindByUserId(string userId)
        {
            Query.Where(x => x.UserId == userId);
        }
    }
}
