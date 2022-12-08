using CoreBridge.Models.DTO;
using CoreBridge.Models.DTO.Requests;
using CoreBridge.Models.Entity;
using CoreBridge.Models.Exceptions;
using CoreBridge.Models.Interfaces;
using CoreBridge.Services.Interfaces;
using MessagePack;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace CoreBridge.Services
{
    public class SessionStatusService : ISessionStatusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHashService _hash;
        private readonly IUserService _userSvc;
        private readonly ITitleInfoService _titleInfoService;
        private readonly IDistributedCache _cache;

        public SessionStatusService(IUnitOfWork unitOfWork, IHashService hash,
            ITitleInfoService titleInfo, IUserService user,
            IDistributedCache cache)
        {
            _unitOfWork = unitOfWork;
            _hash = hash;
            _titleInfoService = titleInfo;
            _userSvc = user;
            _cache = cache;
        }
        //SessionStatusAdminMiddlewareでイニシャライズ
        public bool IsClientApi { get; set; }
        public bool IsServerApi { get; set; }
        public bool UseJson { get; set; }
        public bool UseHash { get { return IsServerApi; } }

        //BaseController.SetParamsでイニシャライズ
        public string? TitleCode { get; set; }
        public string? SessionKey { get; set; }
        public string ReqUri { get; set; }
        public int? Platform { get; set; }
        public int? SkuType { get; set; }
        public string? UserId { get; set; }
        public int ApiCode { get; set; }

        public TitleInfoDto TitleInfo { get; set; }
        public GFSUserDto UserInfo { get; set; }

        public object RequestBody
        {
            get
            {
                if (UseJson) return JsonRequest;
                else return MsgPackRequest;
            }
            set
            {
                if (UseJson)
                {
                    JsonRequest = (string)value;
                }
                else
                {
                    MsgPackRequest = (byte[])value;
                }
            }
        }
        public string? RequestBodyStr
        {
            get
            {
                if (UseJson) return JsonRequest;
                else return MsgPackRequestInJson;
            }
        }
        public string? ResponseBodyStr
        {
            get
            {
                if (UseJson) return JsonResponse;
                else return MsgPackResponseInJson;
            }
        }
        public object ResponseBody
        {
            get
            {
                if (UseJson) return JsonResponse;
                else return MsgPackResponse;
            }
            set
            {
                if (UseJson)
                {
                    JsonResponse = (string)value;
                }
                else
                {
                    MsgPackResponse = (byte[])value;
                }
            }
        }
        public string? JsonRequest { get; set; } = null;
        public string? JsonResponse { get; set; } = null;
        public byte[]? MsgPackRequest { get; set; } = null;
        public byte[]? MsgPackResponse { get; set; } = null;
        public byte[]? RequestHash { get; set; } = null;
        public string MsgPackRequestInJson
        {
            get
            {
                return JsonSerializer.Serialize(MessagePackSerializer.Deserialize<object[]>(MsgPackRequest));
            }
        }
        public string MsgPackResponseInJson
        {
            get
            {
                return JsonSerializer.Serialize(MessagePackSerializer.Deserialize<object[]>(MsgPackResponse));
            }
        }
        public ReqBase ReqParam { get; set; } = null;
        public ReqBase ReqHeader { get; set; } = null;


        /// <summary>
        /// copy request body from req and save to Json/MsgPackRequestbody
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task CopyRequestBody(HttpRequest req)
        {
            byte[] originalContent;
            using (StreamReader stream = new StreamReader(req.Body))
            {
                var ms = new MemoryStream();
                await stream.BaseStream.CopyToAsync(ms);
                originalContent = ms.ToArray();
            }

            if (UseHash)
            {
                RequestHash = originalContent.Take(16).ToArray();
                originalContent = originalContent.Skip(16).ToArray();
            }

            if (UseJson)
            {
                JsonRequest = originalContent.ToString();
            }
            else
            {
                MsgPackRequest = originalContent;
            }
            req.Body = new MemoryStream(originalContent);
        }

        /// <summary>
        /// copy request body from res and save to Json/MsgPackResponsebody
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task CopyResponseBody(HttpResponse res)
        {
            byte[] originalContent;
            using (StreamReader stream = new StreamReader(res.Body))
            {
                var ms = new MemoryStream();
                await stream.BaseStream.CopyToAsync(ms);
                originalContent = ms.ToArray();
            }

            if (UseHash)
            {
                originalContent = originalContent.Skip(16).ToArray();
            }

            if (UseJson)
            {
                JsonResponse = originalContent.ToString();
            }
            else
            {
                MsgPackResponse = originalContent;
            }
            res.Body = new MemoryStream(originalContent);
        }

        public async Task LoadTitleInfo(string titleCode)
        {
            TitleInfo = await _titleInfoService.GetByCodeAsync(titleCode);
            if (TitleInfo == null)
            {
                throw new BNException(ApiCode, BNException.BNErrorCode.TitleCodeInvalid,
                    BNException.ErrorLevel.Error, $"不正なタイトルコードが指定されました。title_cd[{TitleCode}]");
            }
        }

        public async Task LoadUserInfo()
        {
            UserInfo = await _userSvc.LoadCurrentUser();

        }

#if DEBUG
        public async Task SaveSessionDebugInfo()
        {
            DebugInfo info = new DebugInfo
            {
                TitleCode = this.TitleCode,
                UserId = this.UserId,
                RequestBody = this.RequestBodyStr,
                ResponseBody = this.ResponseBodyStr
            };
            info.SetPrimaryKey();
            await _unitOfWork.DebugInfoRepository.AddAsync(info);
            await _unitOfWork.CommitAsync();
        }
#endif

    }
}
