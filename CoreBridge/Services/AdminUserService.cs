using CoreBridge.Models.DTO;
using CoreBridge.Models.Entity;
using CoreBridge.Models.Interfaces;
using CoreBridge.Services.Interfaces;
using CoreBridge.Specifications;

namespace CoreBridge.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<AdminUserService> _logger;

        public AdminUserService(IUnitOfWork unitOfWork, ILogger<AdminUserService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<List<AdminUserDto>> ListAsync()
        {
            List<AdminUserDto> result = new();
            var list = await _unitOfWork.AdminUserRepository.ListAsync();
            foreach(AdminUser entity in list)
            {
                result.Add(new(entity.Name, entity.EMail,entity.Password,entity.Password));
            }

            return result;
        }

        public async Task<AdminUserDto> GenerateAdminUser(AdminUserDto dto)
        {

            AdminUser entity = new(dto.Name, dto.EMail, dto.Password);

            await _unitOfWork.AdminUserRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            return dto;
        }

        public async Task<AdminUserDto> LoginAdminUser(AdminUserDto dto)
        {
            AdminUserSpecification spec = new();
            spec.FindByEmail(dto.EMail);

            AdminUser entity = await _unitOfWork.AdminUserRepository.GetBySpecAsync(spec);
            AdminUserDto result = null;
            if (IsValidLogin(dto, entity))
                result = new(entity.Name, entity.EMail, entity.Password, entity.Password);

            return result;
        }

        private bool IsValidLogin(AdminUserDto dto, AdminUser entity)
        {

            if (dto == null || entity == null)
                return false;

            if (dto.EMail != entity.EMail)
                return false;


            if (dto.Password != entity.Password)
                return false;

            return true;
        }
    }
}
