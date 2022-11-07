using Ardalis.Specification;
using CoreBridge.Models.Entity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CoreBridge.Specifications
{
    public class AdminUserSpecification : Specification<AdminUser>
    {
        public AdminUserSpecification(string? id = null)
        {
            if (id != null)
                FindById(id);
        }

        public void FindById(string id)
        {
            Query.Where(x => x.Id == id);
        }

        public void FindByEmail(string EMail)
        {
            Query.Where(x => x.EMail == EMail);
        }
    }
}
