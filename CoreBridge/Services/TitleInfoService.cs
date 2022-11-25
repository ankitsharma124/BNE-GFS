using AutoMapper;
using CoreBridge.Models.DTO;
using CoreBridge.Models.Interfaces;
using CoreBridge.Services.Interfaces;

namespace CoreBridge.Services
{
    public class TitleInfoService : ITitleInfoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TitleInfoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<TitleInfoDto> GetByCodeAsync(string titleCode)
        {
            var entity = await _unitOfWork.TitleInfoRepository.GetByIdAsync(titleCode);
            return _mapper.Map<TitleInfoDto>(entity);

        }
    }
}
