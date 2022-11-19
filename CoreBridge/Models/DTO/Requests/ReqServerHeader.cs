namespace CoreBridge.Models.DTO.Requests
{
    public class ReqServerHeader : ReqBaseClientServerParamHeader
    {
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
