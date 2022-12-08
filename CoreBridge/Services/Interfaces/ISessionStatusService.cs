using CoreBridge.Models.DTO;
using CoreBridge.Models.DTO.Requests;

namespace CoreBridge.Services.Interfaces
{
    public interface ISessionStatusService
    {
        int ApiCode { get; set; }
        bool IsClientApi { get; set; }
        bool IsServerApi { get; set; }
        string? JsonRequest { get; set; }
        string? JsonResponse { get; set; }
        byte[]? MsgPackRequest { get; set; }
        string MsgPackRequestInJson { get; }
        byte[]? MsgPackResponse { get; set; }
        string MsgPackResponseInJson { get; }
        int? Platform { get; set; }
        ReqBase ReqHeader { get; set; }
        ReqBase ReqParam { get; set; }
        object RequestBody { get; set; }
        string? RequestBodyStr { get; }
        byte[]? RequestHash { get; set; }
        string ReqUri { get; set; }
        object ResponseBody { get; set; }
        string? ResponseBodyStr { get; }
        string? SessionKey { get; set; }
        int? SkuType { get; set; }
        string? TitleCode { get; set; }
        TitleInfoDto TitleInfo { get; set; }
        bool UseHash { get; }
        bool UseJson { get; set; }
        string? UserId { get; set; }
        GFSUserDto UserInfo { get; set; }

        Task CopyRequestBody(HttpRequest req);
        Task CopyResponseBody(HttpResponse res);
        Task LoadTitleInfo(string titleCode);
        Task LoadUserInfo();
        Task SaveSessionDebugInfo();
    }
}