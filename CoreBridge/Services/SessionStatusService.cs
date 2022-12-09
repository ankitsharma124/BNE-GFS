using CoreBridge.Models;
using CoreBridge.Models.DTO;
using CoreBridge.Models.DTO.Requests;
using CoreBridge.Models.Entity;
using CoreBridge.Models.Exceptions;
using CoreBridge.Models.Extensions;
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

        public bool IsBnIdApi { get; set; } = false; //どこでイニシャライズ？
        public bool UseJson { get; set; }
        public bool UseHash { get { return IsServerApi; } }

        //ControllerActionでイニシャライズ
        public string? TitleCode { get; set; }
        public string? SessionKey { get; set; }
        public string ReqPath { get; set; }
        public int? Platform { get; set; }
        public int? SkuType { get; set; }
        public string? UserId { get; set; }
        public int ApiCode { get; set; }
        public ReqBase ReqParam { get; set; } = null;
        public ReqBase ReqHeader { get; set; } = null;
        public TitleInfoDto TitleInfo { get; set; }
        public GFSUserDto UserInfo { get; set; }



        //Msgpack/jsonの選別をして、保管してあるbodyのコピーを返す
        public object? RequestBody
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
        public object? ResponseBody
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

        //msgpack/jsonに関わらずjsonString化して、保管してあるbodyのコピーを返す
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

        //msgpackをjsonにして返す
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

        //underlying data - SessionStatusAdminMiddlewareでイニシャライズ
        public string? JsonRequest { get; set; } = null;
        public string? JsonResponse { get; set; } = null;
        public byte[]? MsgPackRequest { get; set; } = null;
        public byte[]? MsgPackResponse { get; set; } = null;
        public byte[]? RequestHash { get; set; } = null;

        public IQueryCollection Query { get; set; } = null;//BnId controller only

        public void CopyParamHeader()
        {
            if (!ReqHeader.IsOrDescendantOf(typeof(ReqBaseClientServerParamHeader)))
            {
                throw new Exception("smtg wrong with the header");
            }

            var header = (ReqBaseClientServerParamHeader)ReqHeader;
            TitleCode = header.TitleCd;
            UserId = header.UserId;
            SkuType = header.SkuType;
            SessionKey = header.Session;
            Platform = header.Platform;
        }

        /// <summary>
        /// copy request body from req and save to Json/MsgPackRequestbody
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task CopyRequestBody(HttpRequest req)
        {
            if (req.Method.ToUpper() == "GET")
            {
                Query = req.Query;
            }
            else
            {
                byte[] originalContent;
                using (StreamReader stream = new StreamReader(req.Body))
                {
                    var ms = new MemoryStream();
                    await stream.BaseStream.CopyToAsync(ms);
                    originalContent = ms.ToArray();
                }
                this.ReqPath = req.Path.ToString();
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
        /// <summary>
        /// phpのBnIdController.entry_userdataに相当
        /// </summary>
        /// <param name="info"></param>
        public void CopyBnIdUserInfo(BnIdTempInfo info)
        {
            TitleCode = info.TitleCode;
            UserId = info.UserId;
            SkuType = info.SkuType;
            Platform = info.Platform;
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
            UserInfo = await _userSvc.GetByIdAsync(UserId);
        }

#if DEBUG
        public async Task SaveSessionDebugInfo()
        {
            string reqBody;
            if (Query == null) //for BnIdController calls
            {
                reqBody = JsonSerializer.Serialize(Query);
            }
            else
            {
                reqBody = this.RequestBodyStr;
            }

            DebugInfo info = new DebugInfo
            {
                TitleCode = this.TitleCode,
                UserId = this.UserId,
                RequestBody = reqBody,
                ResponseBody = this.ResponseBodyStr,
                RequestPath = this.ReqPath
            };
            info.SetPrimaryKey();
            await _unitOfWork.DebugInfoRepository.AddAsync(info);
            await _unitOfWork.CommitAsync();
        }
#endif

    }
}
