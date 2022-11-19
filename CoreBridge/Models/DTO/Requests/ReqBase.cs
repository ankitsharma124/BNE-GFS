namespace CoreBridge.Models.DTO.Requests
{
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

        public virtual int ApiCode { get { return 9999; } }

        //public HttpRequest? HttpReqObj { get; set; } = null;
    }
}
