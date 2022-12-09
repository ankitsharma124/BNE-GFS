using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace CoreBridge.Models.DTO.Requests
{
    public class TestParam : ReqBase
    {
        public string name { get; set; }
    }

    public class BnIdTestParam : ReqBase
    {
        [FromQuery]
        public string seq { get; set; }
    }

    public class ClientTestParam : ReqBaseClient
    {
        public string Name { get; set; }
    }
}
