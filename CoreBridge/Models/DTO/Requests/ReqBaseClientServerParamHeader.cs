using CoreBridge.Models.Exceptions;
using MessagePack;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CoreBridge.Models.DTO.Requests
{
    [MessagePackObject]
    public class ReqBaseClientServerParamHeader : ReqBaseParamHeader
    {
        [Key(10)]
        [FromBody]
        public string? TitleCode { get; set; } = null;
        [Key(11)]
        [FromBody]
        public string? UserId { get; set; } = null;
        [Key(12)]
        [FromBody]
        public int? SkuType { get; set; } = null;
        [Key(13)]
        [FromBody]
        public string? Session { get; set; } = null;
        [Key(14)]
        [FromBody]
        public int? Platform { get; set; } = null;

        [IgnoreMember]
        public HttpRequest? HttpReqObj { get; set; } = null;

        public void Validate()
        {
            if (TitleCode == null || UserId == null || SkuType == null || Session == null || Platform == null)
            {
                throw new BNException(0, BNException.BNErrorCode.ParamExists,
                    "ヘッダー項目が足りません。" + Environment.NewLine + JsonConvert.SerializeObject(HttpReqObj));
            }
            if (TitleCode == "" || UserId == "" || SkuType == 0 || Session == "" || Platform == 0)
            {
                throw new BNException(0, BNException.BNErrorCode.ParamType,
                    "ヘッダー項目型エラー。" + Environment.NewLine + JsonConvert.SerializeObject(HttpReqObj));
            }
            if (Array.IndexOf(SysConsts.PlatformList, Platform) < 0)
            {
                throw new BNException(0, BNException.BNErrorCode.PlatformTypeInvalid,
                    "範囲外エラー:プラットフォーム種別が指定範囲外です");
            }
            if (SkuType != (int)SysConsts.SkuType.Product && SkuType != (int)SysConsts.SkuType.Trial)
            {
                throw new BNException(0, BNException.BNErrorCode.OutOfRange,
                    "範囲外エラー:SKU種別は0か1で指定してください");
            }

        }




    }
}
