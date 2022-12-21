using CoreBridge.Models.DTO;
using CoreBridge.Models.DTO.Requests;
using CoreBridge.Models.Entity;

namespace CoreBridge.Services.Interfaces
{
    public interface ISessionStatusService
    {
        int ApiCode { get; set; }
        bool IsBnIdApi { get; set; }
        bool IsClientApi { get; set; }
        bool IsServerApi { get; set; }
        string? JsonRequest { get; set; }
        string? JsonResponse { get; set; }
        byte[]? MsgPackRequest { get; set; }
        string MsgPackRequestInJson { get; }
        byte[]? MsgPackResponse { get; set; }
        string MsgPackResponseInJson { get; }
        int? Platform { get; set; }
        IQueryCollection Query { get; set; }
        ReqBase ReqHeader { get; set; }
        ReqBase ReqParam { get; set; }
        string ReqPath { get; set; }
        object? RequestBody { get; set; }
        string? RequestBodyStr { get; }
        byte[]? RequestHash { get; set; }
        object? ResponseBody { get; set; }
        string? ResponseBodyStr { get; }
        string? Session { get; set; }
        int? SkuType { get; set; }
        string? TitleCode { get; set; }
        TitleInfoDto TitleInfo { get; set; }
        bool UseHash { get; }
        bool UseJson { get; set; }
        string? UserId { get; set; }
        GFSUserDto UserInfo { get; set; }

        void CopyBnIdUserInfo(BnIdTempInfo info);

#if DEBUG
        Task SaveSessionDebugInfo();
#endif

    }
}
