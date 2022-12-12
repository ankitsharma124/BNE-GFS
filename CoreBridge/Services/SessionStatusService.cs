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
        private readonly IUnitOfWork _unit;
        public SessionStatusService(IUnitOfWork unit)
        {
            _unit = unit;
        }
        //SessionStatusAdminMiddlewareでイニシャライズ
        public bool IsClientApi { get; set; }
        public bool IsServerApi { get; set; }

        public bool IsBnIdApi { get; set; } = false; //どこでイニシャライズ？
        public bool UseJson { get; set; }
        public bool UseHash { get { return IsServerApi; } }

        //ControllerActionでイニシャライズ
        public string? TitleCode { get; set; }
        public string? Session { get; set; }
        public string ReqPath { get; set; }
        public int? Platform { get; set; }
        public string PlatformStr { get; set; }
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
            await _unit.DebugInfoRepository.AddAsync(info);
            await _unit.CommitAsync();
        }
#endif

    }
}
