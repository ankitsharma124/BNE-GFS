namespace CoreBridge.Models.DTO.Requests
{
    public class ReqBag<THeader, TParam>
    {
        public THeader Header { get; set; }
        public TParam Param { get; set; }

    }
}
