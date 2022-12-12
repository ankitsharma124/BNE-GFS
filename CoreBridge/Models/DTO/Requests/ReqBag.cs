using MessagePack;

namespace CoreBridge.Models.DTO.Requests
{
    [MessagePackObject]
    public class ReqBag<THeader, TParam>
    {
        [Key(0)]
        public THeader Header { get; set; }
        [Key(1)]
        public TParam Param { get; set; }

    }
}
