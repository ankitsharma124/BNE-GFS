namespace CoreBridge.Models.DTO.Requests
{
    public abstract class ReqBase
    {
        protected object _data;

        public static object[] GetRules(bool added = false)
        {
            return new object[] { };
        }

        public ReqBase(object data = null)
        {
            _data = data;
        }

        public void Validate()
        {
            this.Convert();
            //todo? log validation complete
        }

        public abstract void Convert();


    }
}
