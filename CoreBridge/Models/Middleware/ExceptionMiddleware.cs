using CoreBridge.Models.Exceptions;
using CoreBridge.Services.Interfaces;
using Google.Protobuf.WellKnownTypes;
using System.Net;
using Microsoft.Extensions.Primitives;
using System.Net.Mime;
using Newtonsoft.Json;
using MessagePack;
using Microsoft.AspNetCore.Http;
using Google.Api;

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
            catch (Manual404 m404)
            {
                _logger.LogError(m404.Message);
                //CustomResponseを返す？　返さない？　(Q5)
                //BNのHeaderが返ってくるかどうかテストするクライアントとかいる？
                //await HandleExceptionAsync(httpContext, 404);

                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                var responseContent = new
                {
                    StatusCode = httpContext.Response.StatusCode
                };
                await httpContext.Response.WriteAsJsonAsync(responseContent);



            }
            catch (Exception ex)
            {
                _logger.LogError("Raw error", ex);
                await HandleExceptionAsync(httpContext, 99990001);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, int statusCode)
        {
            await _responseService.ReturnBNErrorAsync(context.Response, statusCode);
        }

    }
}
