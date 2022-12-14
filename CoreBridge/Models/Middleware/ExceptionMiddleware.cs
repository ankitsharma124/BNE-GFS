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
        private ILogger<ExceptionMiddleware>? _logger;
        private IResponseService? _responseService;
        //todo: ResponseServiceにBaseControllerForMsgPackとの共通機能を集約
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext, ILogger<ExceptionMiddleware> logger, IResponseService responseService)
        {
            _logger = logger;
            _responseService = responseService;

            try
            {
                await _next(httpContext);
            }
#if DEBUG
            catch (DebugException ex)
            {
                _logger.LogError(ex, "[Debug Err]");
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var responseContent = new
                {
                    StatusCode = httpContext.Response.StatusCode,
                    ErrorMessage = ex.Message
                };
                await httpContext.Response.WriteAsJsonAsync(responseContent);
            }
#endif
            catch (BNException bnx)
            {
                if ((int)bnx.Level >= (int)BNException.ErrorLevel.Error)
                {
                    _logger.LogError(bnx, bnx.Code.ToString() + $" | StatusCode[{bnx.StatusCode}]");
                }
#if DEBUG
                else if ((int)bnx.Level >= (int)BNException.ErrorLevel.Info)
                {
                    _logger.LogInformation(bnx, bnx.Code.ToString() + $" | StatusCode[{bnx.StatusCode}]");
                }
                else
                {
                    _logger.LogDebug(bnx, bnx.Code.ToString() + $" | StatusCode[{bnx.StatusCode}]");
                }
#else

                _logger.LogError(bnx, bnx.Code.ToString() + $" | StatusCode[{bnx.StatusCode}]");
#endif
                await HandleExceptionAsync(httpContext, bnx.StatusCode);
            }
            catch (Manual404 m404)
            {
                //_logger.LogError(m404.Message);
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
                throw;
                //await HandleExceptionAsync(httpContext, 99990001);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, int statusCode)
        {
            await _responseService.ReturnBNErrorAsync(context.Response, statusCode);
        }

    }
}
