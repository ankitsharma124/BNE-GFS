using Ardalis.Specification;
using CoreBridge.Models;
using CoreBridge.Models.Exceptions;
using CoreBridge.Services.Interfaces;
using Google.Api;
using Google.Type;
using MessagePack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Google.Rpc.Context.AttributeContext.Types;
using DateTime = System.DateTime;

namespace CoreBridge.Controllers.api
{
    public class BaseControllerForMsgPack : ControllerBase
    {
        private readonly IResponseService _responseService;
        protected IHostEnvironment _env;
        public BaseControllerForMsgPack(IHostEnvironment env, IResponseService responseService)
        {
            _env = env;
            _responseService = responseService;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="details">基本、object[]で送り込む方向, details[0]はstatusCode</param>
        /// <param name="result"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        protected async Task ReturnBNResponse(object details, int result = -1, int status = -1)
        {
            await _responseService.ReturnBNResponseAsync(Response, details, CustomizeResponseInnerHeader,
                CustomizeResponseContent, result, status);
        }

        protected void CustomizeResponseInnerHeader(object[] customHeader)
        {
            //必要に応じて継承クラスでoverride
            //php BaseControllerの_response_custom_headerに相当
        }

        protected void CustomizeResponseContent(object response)
        {
            //必要に応じて継承クラスでoverride
            //php BaseControllerの_response_custom_dataに相当
        }

    }
}
