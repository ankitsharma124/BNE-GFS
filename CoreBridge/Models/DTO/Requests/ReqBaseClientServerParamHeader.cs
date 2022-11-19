using CoreBridge.Models.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CoreBridge.Models.DTO.Requests
{
    public class ReqBaseClientServerParamHeader : ReqBaseParam
    {

        [FromHeader]
        public string? TitleCd { get; set; } = null;
        [FromHeader]
        public string? UserId { get; set; } = null;
        [FromHeader]
        public int? SkuType { get; set; } = null;
        [FromHeader]
        public string? Session { get; set; } = null;
        [FromHeader]
        public int? Platform { get; set; } = null;


        public HttpRequest? HttpReqObj { get; set; } = null;

        public void Validate()
        {
            if (TitleCd == null || UserId == null || SkuType == null || Session == null || Platform == null)
            {
                throw new BNException(0, BNException.BNErrorCode.ParamExists,
                    "ヘッダー項目が足りません。" + Environment.NewLine + JsonConvert.SerializeObject(HttpReqObj));
            }
            if (TitleCd == "" || UserId == "" || SkuType == 0 || Session == "" || Platform == 0)
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
