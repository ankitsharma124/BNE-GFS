using MessagePack;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace CoreBridge.Models.DTO.Requests
{
    [MessagePackObject]
    public class TestParam : ReqBase
    {
        [Key(100)]
        public string name { get; set; }
    }
    [MessagePackObject]
    public class BnIdTestParam : ReqBase
    {
        [Key(100)]
        [FromQuery]
        public string seq { get; set; }
    }

    [MessagePackObject]
    public class ClientTestParam : ReqBaseClient
    {
        [Key(100)]
        public string Name { get; set; }
    }

    [MessagePackObject]
    public class ServerTestParam : ReqBaseServer
    {
        [Key(100)]
        public string Name { get; set; }
    }
}
