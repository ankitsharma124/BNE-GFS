using Ardalis.Specification;
using CoreBridge.Models.Entity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CoreBridge.Specifications
{
    public class TitleInfoSpecification : Specification<TitleInfo>
    {
        public TitleInfoSpecification(string? id = null)
        {
            if (id != null)
                FindById(id);
        }

        public void FindById(string id)
        {
            Query.Where(x => x.Id == id);
        }
    }
}
