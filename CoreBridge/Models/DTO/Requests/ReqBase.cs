using MessagePack;

namespace CoreBridge.Models.DTO.Requests
{
    [MessagePackObject]
    public abstract class ReqBase
    {


        public static object[] GetRules(bool added = false)
        {
            return new object[] { };
        }

        public ReqBase()
        {

        }

        public void Validate()
        {
            //todo? log validation complete
        }


        //public HttpRequest? HttpReqObj { get; set; } = null;
    }
}
