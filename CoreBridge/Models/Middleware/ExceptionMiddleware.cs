using CoreBridge.Models.Exceptions;
using CoreBridge.Services.Interfaces;
using Google.Protobuf.WellKnownTypes;
using System.Net;
using Microsoft.Extensions.Primitives;
using System.Net.Mime;
using Newtonsoft.Json;
using MessagePack;
using Microsoft.AspNetCore.Http;

namespace CoreBridge.Models.Middleware
{
    /// <summary>
    /// エラーをキャッチ、エラーレスポンスを作成し送り返す。
    /// </summary>
    public class ExceptionMiddleware
    {


        private readonly RequestDelegate _next;
        private ILoggerService? _logger;
        private IResponseService? _responseService;
        //todo: ResponseServiceにBaseControllerForMsgPackとの共通機能を集約
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext, ILoggerService logger, IResponseService responseService)
        {
            _logger = logger;
            _responseService = responseService;

            try
            {
                await _next(httpContext);
            }
            catch (BNException bnx)
            {
                _logger.LogError(bnx);
                await HandleExceptionAsync(httpContext, bnx.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError("Raw error", ex);
                await HandleExceptionAsync(httpContext, httpContext.Response.StatusCode);
            }
        }


        private async Task HandleExceptionAsync(HttpContext context, int statusCode)
        {

            _responseService.ReturnBNErrorAsync(context.Response, statusCode);

        }

    }
}
