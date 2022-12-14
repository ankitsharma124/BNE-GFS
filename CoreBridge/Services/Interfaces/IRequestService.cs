using CoreBridge.Models.DTO.Requests;

namespace CoreBridge.Services.Interfaces
{
    public interface IRequestService
    {
        /// <summary>
        /// ApiActionの下準備の各種バリデーションやセキュリティチェックを行う。
        /// 各ApiActionにて、まずsssのApiCodeを設定し、
        /// その直後にコールすべし。
        /// </summary>
        /// <param name="req"></param>
        /// <param name="reqHeader"></param>
        /// <param name="reqParam"></param>
        /// <returns></returns>
        Task ProcessRequest(HttpRequest req, ReqBase reqHeader, ReqBaseParam reqParam);
        void LoadStatus_ParamHeader();
        Task LoadStatus_RequestBody(HttpRequest req);
    }
}