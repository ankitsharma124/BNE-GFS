using CoreBridge.Models.Entity;
using CoreBridge.Models.DTO;

namespace CoreBridge.ViewModels
{
    public class MUserIndexViewModel
    {
        public ManagementUserDto ManagementUserDto { get; set; }
        public IEnumerable<ManagementUserDto> ManagementUserDtos { get; set; }

    }
}
