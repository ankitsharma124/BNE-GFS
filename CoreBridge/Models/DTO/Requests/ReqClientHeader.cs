using CoreBridge.Models.Exceptions;
using MessagePack;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;

namespace CoreBridge.Models.DTO.Requests
{
    [MessagePackObject]
    public class ReqClientHeader : ReqBaseClientServerParamHeader
    {

        public ReqClientHeader() : base()
        {
            //???should validate after construction (let controller method run)???
            //this.Validate();
        }

        /// <summary>
        /// バリデーションスキーマの取得用とのこと、詳細不明
        /// </summary>
        /// <param name="added"></param>
        /// <returns></returns>
        public static object[] GetRules(bool added = false)
        {
            return new object[] { };
        }
    }
}
